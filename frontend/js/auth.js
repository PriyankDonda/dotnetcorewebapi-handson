// API Configuration
const API_CONFIG = {
    BASE_URL: 'https://localhost:5001/api',
    TIMEOUT: 10000, // 10 seconds
    ENDPOINTS: {
        AUTH: {
            LOGIN: '/auth/login',
            REGISTER: '/auth/register'
        },
        USER: {
            PROFILE: '/users/profile',
            LIST: '/users'
        }
    }
};

// Auth Service Module
const AuthService = {
    // API endpoints
    endpoints: {
        login: `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.LOGIN}`,
        register: `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.AUTH.REGISTER}`,
        profile: `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.USER.PROFILE}`,
        allUsers: `${API_CONFIG.BASE_URL}${API_CONFIG.ENDPOINTS.USER.LIST}`
    },

    // Current user data
    currentUser: null,

    // Get auth headers
    getAuthHeaders() {
        const token = localStorage.getItem('token');
        if (!token) {
            throw new Error('No token found');
        }
        return {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
            'Accept': 'application/json'
        };
    },

    // Check if user is logged in
    isLoggedIn() {
        return localStorage.getItem('token') !== null;
    },

    // Generic fetch wrapper with timeout
    async fetchWithTimeout(url, options = {}) {
        const controller = new AbortController();
        const timeoutId = setTimeout(() => controller.abort(), API_CONFIG.TIMEOUT);

        try {
            const response = await fetch(url, {
                ...options,
                signal: controller.signal
            });
            clearTimeout(timeoutId);
            return response;
        } catch (error) {
            clearTimeout(timeoutId);
            if (error.name === 'AbortError') {
                throw new Error('Request timeout');
            }
            throw error;
        }
    },

    // Login user
    async login(username, password) {
        try {
            const response = await this.fetchWithTimeout(this.endpoints.login, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Login failed');
            }

            const data = await response.json();
            if (!data.token) {
                throw new Error('No token received from server');
            }

            localStorage.setItem('token', data.token);
            return data;
        } catch (error) {
            console.error('Login error:', error);
            throw error;
        }
    },

    // Register new user
    async register(username, password) {
        try {
            const response = await this.fetchWithTimeout(this.endpoints.register, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Registration failed');
            }

            return await response.json();
        } catch (error) {
            console.error('Registration error:', error);
            throw error;
        }
    },

    // Logout user
    logout() {
        localStorage.removeItem('token');
        this.currentUser = null;
    },

    // Get user profile
    async getProfile() {
        try {
            const token = localStorage.getItem('token');
            if (!token) {
                throw new Error('Not authenticated');
            }

            const response = await this.fetchWithTimeout(this.endpoints.profile, {
                method: 'GET',
                headers: this.getAuthHeaders()
            });

            const responseText = await response.text();
            if (!responseText) {
                throw new Error('Empty response from server');
            }

            let data;
            try {
                data = JSON.parse(responseText);
            } catch (e) {
                console.error('JSON parse error:', e);
                throw new Error('Invalid JSON response from server');
            }

            if (!response.ok) {
                if (response.status === 401) {
                    this.logout();
                    throw new Error('Session expired. Please login again.');
                }
                throw new Error(data.message || 'Failed to get profile');
            }
            
            if (!data) {
                throw new Error('No profile data received');
            }

            this.currentUser = data;
            return data;
        } catch (error) {
            console.error('Profile error:', error);
            this.logout();
            throw error;
        }
    },

    // Get all users
    async getAllUsers() {
        try {
            const token = localStorage.getItem('token');
            if (!token) {
                throw new Error('Not authenticated');
            }

            const response = await this.fetchWithTimeout(this.endpoints.allUsers, {
                method: 'GET',
                headers: this.getAuthHeaders()
            });

            if (!response.ok) {
                const error = await response.json();
                throw new Error(error.message || 'Failed to get users');
            }

            return await response.json();
        } catch (error) {
            console.error('Users error:', error);
            throw error;
        }
    }
}; 