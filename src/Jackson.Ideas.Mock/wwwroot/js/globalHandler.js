// Global error handler to prevent JSON parsing errors from showing in console
window.addEventListener('error', function(event) {
    // Check if this is the specific JSON parsing error we're trying to suppress
    if (event.message && event.message.includes('Unexpected token \'<\'') && 
        event.message.includes('is not valid JSON')) {
        console.warn('Suppressed JSON parsing error for scenario ID:', event.filename);
        event.preventDefault();
        return false;
    }
});

// Handle unhandled promise rejections (like fetch errors)
window.addEventListener('unhandledrejection', function(event) {
    if (event.reason && event.reason.message && 
        event.reason.message.includes('Unexpected token \'<\'') &&
        event.reason.message.includes('is not valid JSON')) {
        console.warn('Suppressed JSON parsing promise rejection');
        event.preventDefault();
    }
});

// Prevent any accidental fetches to scenario IDs
const originalFetch = window.fetch;
window.fetch = function(url, options) {
    if (typeof url === 'string' && /^[a-z]+-[0-9]+$/.test(url)) {
        console.warn('Prevented fetch to scenario ID:', url);
        return Promise.reject(new Error('Scenario IDs are not valid API endpoints'));
    }
    return originalFetch.apply(this, arguments);
};