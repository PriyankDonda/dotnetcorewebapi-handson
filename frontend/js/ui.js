// UI Service Module
const UIService = {
    elements: {
        loginForm: null,
        registerForm: null,
        profile: null,
        dashboard: null,
        notification: null,
        loginBtn: null,
        registerBtn: null,
        logoutBtn: null,
        homeLink: null,
        profileLink: null,
        usersLink: null,
        loadingIndicator: null,
        userList: null,
        authButtons: null,
        userItemTemplate: null,
        userListContainer: null
    },

    templates: {
        userItem: null,
        userList: null
    },

    init() {
        // Cache DOM elements
        this.elements.loginForm = document.getElementById('loginForm');
        this.elements.registerForm = document.getElementById('registerForm');
        this.elements.profile = document.getElementById('profile');
        this.elements.dashboard = document.getElementById('dashboard');
        this.elements.loginBtn = document.getElementById('loginBtn');
        this.elements.registerBtn = document.getElementById('registerBtn');
        this.elements.logoutBtn = document.getElementById('logoutBtn');
        this.elements.homeLink = document.querySelector('a[href="#/dashboard"]');
        this.elements.profileLink = document.querySelector('a[href="#/profile"]');
        this.elements.usersLink = document.querySelector('a[href="#/users"]');
        this.elements.loadingIndicator = document.getElementById('loadingIndicator');
        this.elements.userList = document.getElementById('userList');
        this.elements.notification = document.getElementById('notification');
        this.elements.authButtons = document.querySelectorAll('.auth-button');
        this.elements.userItemTemplate = document.getElementById('userItemTemplate');
        this.elements.userListContainer = document.getElementById('userListContainer');

        // Cache templates
        this.templates.userItem = document.getElementById('userItemTemplate');
        this.templates.userList = document.getElementById('userListTemplate');

        // Add event listeners for forms
        if (this.elements.loginForm) {
            const form = this.elements.loginForm.querySelector('form');
            if (form) {
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    const username = document.getElementById('loginUsername').value;
                    const password = document.getElementById('loginPassword').value;
                    App.handleLogin(username, password);
                });
            }
        }

        if (this.elements.registerForm) {
            const form = this.elements.registerForm.querySelector('form');
            if (form) {
                form.addEventListener('submit', (e) => {
                    e.preventDefault();
                    const username = document.getElementById('registerUsername').value;
                    const password = document.getElementById('registerPassword').value;
                    App.handleRegister(username, password);
                });
            }
        }

        // Add click handlers for auth buttons
        if (this.elements.loginBtn) {
            this.elements.loginBtn.addEventListener('click', () => {
                window.location.hash = '#/login';
            });
        }

        if (this.elements.registerBtn) {
            this.elements.registerBtn.addEventListener('click', () => {
                window.location.hash = '#/register';
            });
        }

        if (this.elements.logoutBtn) {
            this.elements.logoutBtn.addEventListener('click', () => {
                App.handleLogout();
            });
        }

        // Update auth buttons visibility based on login state
        this.updateAuthButtons();
    },

    updateAuthButtons() {
        const isLoggedIn = AuthService.isLoggedIn();
        console.log('Updating auth buttons, isLoggedIn:', isLoggedIn); // Debug log

        if (this.elements.loginBtn) this.elements.loginBtn.style.display = isLoggedIn ? 'none' : 'inline-block';
        if (this.elements.registerBtn) this.elements.registerBtn.style.display = isLoggedIn ? 'none' : 'inline-block';
        if (this.elements.logoutBtn) this.elements.logoutBtn.style.display = isLoggedIn ? 'inline-block' : 'none';
        
        // Update navigation links visibility
        if (this.elements.homeLink) this.elements.homeLink.style.display = isLoggedIn ? 'inline-block' : 'none';
        if (this.elements.profileLink) this.elements.profileLink.style.display = isLoggedIn ? 'inline-block' : 'none';
        if (this.elements.usersLink) this.elements.usersLink.style.display = isLoggedIn ? 'inline-block' : 'none';
    },

    showLoginForm() {
        if (this.elements.loginForm) this.elements.loginForm.style.display = 'block';
        if (this.elements.registerForm) this.elements.registerForm.style.display = 'none';
        if (this.elements.profile) this.elements.profile.style.display = 'none';
        if (this.elements.dashboard) this.elements.dashboard.style.display = 'none';
        if (this.elements.userList) this.elements.userList.style.display = 'none';
        this.updateAuthButtons();
    },

    showRegisterForm() {
        if (this.elements.loginForm) this.elements.loginForm.style.display = 'none';
        if (this.elements.registerForm) this.elements.registerForm.style.display = 'block';
        if (this.elements.profile) this.elements.profile.style.display = 'none';
        if (this.elements.dashboard) this.elements.dashboard.style.display = 'none';
        if (this.elements.userList) this.elements.userList.style.display = 'none';
        this.updateAuthButtons();
    },

    showProfile(profile) {
        if (this.elements.loginForm) this.elements.loginForm.style.display = 'none';
        if (this.elements.registerForm) this.elements.registerForm.style.display = 'none';
        if (this.elements.profile) {
            this.elements.profile.style.display = 'block';
            
            // Format the creation date
            const createdDate = new Date(profile.createdAt);
            const formattedDate = createdDate.toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
            });
            
            // Create profile header with username and email
            const header = document.createElement('div');
            header.className = 'profile-header';
            
            const username = document.createElement('h1');
            username.className = 'profile-username';
            username.textContent = profile.username;
            
            const email = document.createElement('div');
            email.className = 'profile-email';
            email.textContent = profile.email;
            
            // Create roles section
            const roles = document.createElement('div');
            roles.className = 'profile-roles';
            profile.roles.forEach(role => {
                const badge = document.createElement('span');
                badge.className = 'role-badge';
                badge.textContent = role;
                roles.appendChild(badge);
            });
            
            header.appendChild(username);
            header.appendChild(email);
            header.appendChild(roles);
            
            // Create profile details section
            const details = document.createElement('div');
            details.className = 'profile-details';
            
            // User ID
            const idGroup = document.createElement('div');
            idGroup.className = 'detail-group';
            idGroup.innerHTML = `
                <div class="detail-label">User ID</div>
                <div class="detail-value">${profile.id}</div>
            `;
            
            // Creation Date
            const dateGroup = document.createElement('div');
            dateGroup.className = 'detail-group';
            dateGroup.innerHTML = `
                <div class="detail-label">Member Since</div>
                <div class="profile-created">${formattedDate}</div>
            `;
            
            details.appendChild(idGroup);
            details.appendChild(dateGroup);
            
            // Clear and update profile content
            this.elements.profile.innerHTML = '';
            this.elements.profile.appendChild(header);
            this.elements.profile.appendChild(details);
        }
        if (this.elements.dashboard) this.elements.dashboard.style.display = 'none';
        if (this.elements.userList) this.elements.userList.style.display = 'none';
        this.updateAuthButtons();
    },

    showDashboard(profile) {
        if (this.elements.loginForm) this.elements.loginForm.style.display = 'none';
        if (this.elements.registerForm) this.elements.registerForm.style.display = 'none';
        if (this.elements.profile) this.elements.profile.style.display = 'none';
        if (this.elements.dashboard) {
            this.elements.dashboard.style.display = 'block';
            
            // Update welcome message
            const welcomeMessage = this.elements.dashboard.querySelector('.dashboard-welcome');
            if (welcomeMessage && profile) {
                welcomeMessage.textContent = `Welcome, ${profile.username}!`;
            }
        }
        if (this.elements.userList) this.elements.userList.style.display = 'none';
        this.updateAuthButtons();
    },

    showUserList(users) {
        if (this.elements.loginForm) this.elements.loginForm.style.display = 'none';
        if (this.elements.registerForm) this.elements.registerForm.style.display = 'none';
        if (this.elements.profile) this.elements.profile.style.display = 'none';
        if (this.elements.dashboard) this.elements.dashboard.style.display = 'none';
        if (this.elements.userList) {
            this.elements.userList.style.display = 'block';
            this.elements.userList.innerHTML = '';

            if (this.templates.userList && this.templates.userItem) {
                const userListTemplate = this.templates.userList.content.cloneNode(true);
                const userListContainer = userListTemplate.querySelector('.user-list-items');

                users.forEach(user => {
                    const userItemTemplate = this.templates.userItem.content.cloneNode(true);
                    const usernameElement = userItemTemplate.querySelector('.user-username');
                    if (usernameElement) {
                        usernameElement.textContent = user.username;
                    }
                    userListContainer.appendChild(userItemTemplate);
                });

                this.elements.userList.appendChild(userListTemplate);
            }
        }
        this.updateAuthButtons();
    },

    showLoading() {
        if (this.elements.loadingIndicator) {
            this.elements.loadingIndicator.style.display = 'flex';
        }
    },

    hideLoading() {
        if (this.elements.loadingIndicator) {
            this.elements.loadingIndicator.style.display = 'none';
        }
    },

    showNotification(message, type = 'success') {
        const notification = this.elements.notification;
        
        // Clear previous content
        notification.innerHTML = '';
        
        // Set notification type
        notification.className = `notification ${type}`;
        
        // Create content element
        const content = document.createElement('div');
        content.className = 'notification-content';
        content.textContent = message;
        
        // Create close button
        const closeButton = document.createElement('button');
        closeButton.className = 'notification-close';
        closeButton.innerHTML = '×';
        closeButton.setAttribute('aria-label', 'Close notification');
        closeButton.onclick = () => this.hideNotification();
        
        // Add elements to notification
        notification.appendChild(content);
        notification.appendChild(closeButton);
        
        // Show notification without affecting layout
        notification.style.display = 'block';
        notification.style.visibility = 'visible';
        
        // Auto-hide after 5 seconds
        setTimeout(() => {
            this.hideNotification();
        }, 5000);
    },

    hideNotification() {
        const notification = this.elements.notification;
        
        // Add hiding class for animation
        notification.classList.add('hiding');
        
        // Remove after animation completes
        setTimeout(() => {
            notification.style.display = 'none';
            notification.style.visibility = 'hidden';
            notification.classList.remove('hiding');
        }, 300);
    }
}; 