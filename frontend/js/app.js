// Main Application Module
const App = {
    init() {
        // Initialize UI first
        UIService.init();
        
        // Initialize router after UI
        Router.init();
    },

    // Handle login form submission
    async handleLogin(username, password) {
        try {
            await AuthService.login(username, password);
            UIService.showNotification('Login successful', 'success');
            window.location.hash = '#/dashboard';
        } catch (error) {
            UIService.showNotification(error.message || 'Login failed', 'error');
        }
    },

    // Handle registration form submission
    async handleRegister(username, password) {
        try {
            await AuthService.register(username, password);
            UIService.showNotification('Registration successful', 'success');
            window.location.hash = '#/login';
        } catch (error) {
            UIService.showNotification(error.message || 'Registration failed', 'error');
        }
    },

    // Handle logout
    handleLogout() {
        AuthService.logout();
        UIService.showNotification('Logged out successfully', 'success');
        window.location.hash = '#/login';
    }
};

// Initialize the application when the DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    App.init();
}); 