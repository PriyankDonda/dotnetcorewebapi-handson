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
            init: () => {
                if (AuthService.isLoggedIn()) {
                    window.location.hash = '#/dashboard';
                } else {
                    UIService.showLoginForm();
                }
            }
        },
        '/register': {
            init: () => {
                if (AuthService.isLoggedIn()) {
                    window.location.hash = '#/dashboard';
                } else {
                    UIService.showRegisterForm();
                }
            }
        },
        '/dashboard': {
            guard: 'auth',
            async init() {
                try {
                    const profile = await AuthService.getProfile();
                    if (!profile) {
                        throw new Error('Failed to load profile');
                    }
                    UIService.showDashboard(profile);
                } catch (error) {
                    throw new Error('Failed to load dashboard: ' + error.message);
                }
            }
        },
        '/users': {
            guard: 'auth',
            async init() {
                try {
                    const users = await AuthService.getAllUsers();
                    if (!Array.isArray(users)) {
                        throw new Error('Invalid users data received');
                    }
                    UIService.showUserList(users);
                } catch (error) {
                    throw new Error('Failed to load users: ' + error.message);
                }
            }
        }
    },

    // Initialize router
    init() {
        window.addEventListener('hashchange', () => this.handleRoute());
        window.addEventListener('load', () => this.handleRoute());
    },

    // Handle route change
    async handleRoute() {
        try {
            this.showLoading();
            const hash = window.location.hash || '#/login';
            const route = hash.slice(1);
            this.currentRoute = route;

            const routeConfig = this.routes[route];
            if (!routeConfig) {
                throw new Error('Route not found');
            }

            // Check route guard
            if (routeConfig.guard) {
                await this.guards[routeConfig.guard]();
            }

            // Initialize route
            if (routeConfig.init) {
                await routeConfig.init();
            }

            this.hideLoading();
        } catch (error) {
            this.handleError(error);
        }
    }
}; 