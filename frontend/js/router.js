// Router Module
const Router = {
    // Current route
    currentRoute: null,

    // Loading state
    isLoading: false,

    // Route guards
    guards: {
        auth: async () => {
            if (!AuthService.isLoggedIn()) {
                throw new Error('Authentication required');
            }
            return true;
        }
    },

    // Show loading indicator
    showLoading() {
        this.isLoading = true;
        UIService.showLoading();
    },

    // Hide loading indicator
    hideLoading() {
        this.isLoading = false;
        UIService.hideLoading();
    },

    // Handle route errors
    handleError(error) {
        console.error('Router error:', error);
        this.hideLoading();
        
        if (error.message === 'Authentication required') {
            AuthService.logout();
            window.location.hash = '#/login';
            return;
        }

        UIService.showNotification(error.message || 'An error occurred', 'error');
    },

    // Route handlers
    routes: {
        '/login': {
            template: 'loginForm',
            init: () => {
                UIService.showLoginForm();
            }
        },
        '/register': {
            template: 'registerForm',
            init: () => {
                UIService.showRegisterForm();
            }
        },
        '/profile': {
            template: 'profile',
            init: async () => {
                try {
                    const profile = await AuthService.getProfile();
                    UIService.showProfile(profile);
                } catch (error) {
                    if (error.message.includes('Access denied')) {
                        UIService.showNotification(error.message, 'error');
                        window.location.hash = '#/dashboard';
                        return;
                    }
                    window.location.hash = '#/login';
                }
            }
        },
        '/dashboard': {
            template: 'dashboard',
            init: async () => {
                try {
                    const profile = await AuthService.getProfile();
                    UIService.showDashboard(profile);
                } catch (error) {
                    if (error.message.includes('Access denied')) {
                        UIService.showNotification(error.message, 'error');
                        window.location.hash = '#/profile';
                        return;
                    }
                    window.location.hash = '#/login';
                }
            }
        },
        '/users': {
            template: 'userList',
            init: async () => {
                try {
                    const users = await AuthService.getAllUsers();
                    UIService.showUserList(users);
                } catch (error) {
                    if (error.message.includes('Access denied')) {
                        UIService.showNotification(error.message, 'error');
                        // Prevent navigation by restoring the previous URL
                        window.history.replaceState(null, '', `#${this.currentRoute || '/dashboard'}`);
                        return;
                    }
                    window.location.hash = '#/login';
                }
            }
        }
    },

    // List of public routes that don't require access control
    publicRoutes: ['/login', '/register'],

    // Initialize router
    init() {
        // Handle initial route
        this.handleRoute();

        // Handle route changes
        window.addEventListener('hashchange', (event) => {
            const newPath = window.location.hash.slice(1);
            
            // If trying to access users page, check access first
            if (newPath === '/users') {
                // Prevent the default navigation
                event.preventDefault();
                
                // Check access without changing URL
                AuthService.getAllUsers()
                    .then(users => {
                        UIService.showUserList(users);
                        this.currentRoute = '/users';
                    })
                    .catch(error => {
                        if (error.message.includes('Access denied')) {
                            UIService.showNotification(error.message, 'error');
                            // Keep the current URL
                            window.history.replaceState(null, '', `#${this.currentRoute || '/dashboard'}`);
                        } else {
                            window.location.hash = '#/login';
                        }
                    });
                return;
            }
            
            this.handleRoute();
        });
    },

    // Helper method to get the appropriate service method for a route
    getServiceMethodForRoute(route) {
        const routeServiceMap = {
            '/profile': AuthService.getProfile,
            '/dashboard': AuthService.getProfile,
            '/users': AuthService.getAllUsers
            // New routes will automatically be protected
        };
        return routeServiceMap[route];
    },

    // Helper method to update UI based on route and data
    updateUIForRoute(route, data) {
        const routeUIMap = {
            '/profile': UIService.showProfile,
            '/dashboard': UIService.showDashboard,
            '/users': UIService.showUserList
            // New routes will automatically be protected
        };
        const updateMethod = routeUIMap[route];
        if (updateMethod) {
            updateMethod(data);
        }
    },

    // Handle route change
    async handleRoute() {
        try {
            UIService.showLoading();
            
            const hash = window.location.hash || '#/login';
            const path = hash.slice(1); // Remove the # symbol
            
            // Check if route exists
            if (!this.routes[path]) {
                throw new Error('Route not found');
            }

            // Check if user is authenticated for protected routes
            if (path !== '/login' && path !== '/register' && !AuthService.isLoggedIn()) {
                window.location.hash = '#/login';
                return;
            }

            // Initialize route
            await this.routes[path].init();
            this.currentRoute = path;
            
        } catch (error) {
            console.error('Router error:', error);
            UIService.showNotification(error.message, 'error');
            if (!AuthService.isLoggedIn()) {
                window.location.hash = '#/login';
            }
        } finally {
            UIService.hideLoading();
        }
    }
}; 