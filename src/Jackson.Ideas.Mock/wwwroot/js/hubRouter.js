/**
 * Hub Router JavaScript functionality for IdeaCoach Pro
 * Handles client-side hub switching, animations, and state management
 */

window.hubRouter = {
    currentHub: 'IdeaDevelopment',
    isTransitioning: false,
    
    /**
     * Called when hub context changes from server-side
     */
    onHubChanged: function(newHub) {
        console.log(`Hub changed from ${this.currentHub} to ${newHub}`);
        
        if (this.isTransitioning) {
            console.log('Hub transition already in progress, skipping');
            return;
        }
        
        this.performHubTransition(this.currentHub, newHub);
        this.currentHub = newHub;
    },
    
    /**
     * Performs smooth transition between hubs
     */
    performHubTransition: function(fromHub, toHub) {
        this.isTransitioning = true;
        
        // Add transition class to body
        document.body.classList.add('hub-transitioning');
        
        // Trigger hub-specific styling changes
        this.applyHubStyling(toHub);
        
        // Show transition animation
        this.showTransitionAnimation(fromHub, toHub);
        
        // Update browser history if needed
        this.updateBrowserState(toHub);
        
        // Clean up after transition
        setTimeout(() => {
            document.body.classList.remove('hub-transitioning');
            this.isTransitioning = false;
        }, 800);
    },
    
    /**
     * Applies hub-specific CSS styling
     */
    applyHubStyling: function(hub) {
        // Remove existing hub classes
        document.body.classList.remove('hub-idea-development', 'hub-business-planning', 'hub-business-operations');
        
        // Add new hub class
        const hubClass = this.getHubCssClass(hub);
        document.body.classList.add(hubClass);
        
        // Update CSS custom properties for smooth color transitions
        this.updateHubColors(hub);
    },
    
    /**
     * Gets CSS class name for hub
     */
    getHubCssClass: function(hub) {
        switch(hub) {
            case 'IdeaDevelopment': return 'hub-idea-development';
            case 'BusinessPlanning': return 'hub-business-planning';
            case 'BusinessOperations': return 'hub-business-operations';
            default: return 'hub-idea-development';
        }
    },
    
    /**
     * Updates CSS custom properties for hub colors
     */
    updateHubColors: function(hub) {
        const root = document.documentElement;
        
        switch(hub) {
            case 'IdeaDevelopment':
                root.style.setProperty('--hub-primary', '#7b8fef');
                root.style.setProperty('--hub-secondary', '#22d3ee');
                root.style.setProperty('--hub-gradient', 'linear-gradient(135deg, #7b8fef 0%, #22d3ee 100%)');
                break;
            case 'BusinessPlanning':
                root.style.setProperty('--hub-primary', '#5a6fd8');
                root.style.setProperty('--hub-secondary', '#3b82f6');
                root.style.setProperty('--hub-gradient', 'linear-gradient(135deg, #5a6fd8 0%, #3b82f6 100%)');
                break;
            case 'BusinessOperations':
                root.style.setProperty('--hub-primary', '#4c5fd7');
                root.style.setProperty('--hub-secondary', '#059669');
                root.style.setProperty('--hub-gradient', 'linear-gradient(135deg, #4c5fd7 0%, #059669 100%)');
                break;
        }
    },
    
    /**
     * Shows transition animation between hubs
     */
    showTransitionAnimation: function(fromHub, toHub) {
        // Create transition overlay
        const overlay = document.createElement('div');
        overlay.className = 'hub-transition-overlay';
        overlay.innerHTML = `
            <div class="transition-content">
                <div class="transition-icon">
                    <i class="${this.getHubIcon(toHub)}"></i>
                </div>
                <div class="transition-text">
                    <h3>Switching to ${this.getHubDisplayName(toHub)}</h3>
                    <p>Loading hub-specific features...</p>
                </div>
                <div class="transition-progress">
                    <div class="progress-bar"></div>
                </div>
            </div>
        `;
        
        document.body.appendChild(overlay);
        
        // Animate progress bar
        setTimeout(() => {
            const progressBar = overlay.querySelector('.progress-bar');
            progressBar.style.width = '100%';
        }, 100);
        
        // Remove overlay after animation
        setTimeout(() => {
            overlay.classList.add('fade-out');
            setTimeout(() => {
                if (overlay.parentNode) {
                    overlay.parentNode.removeChild(overlay);
                }
            }, 300);
        }, 600);
    },
    
    /**
     * Gets icon for hub
     */
    getHubIcon: function(hub) {
        switch(hub) {
            case 'IdeaDevelopment': return 'fas fa-lightbulb';
            case 'BusinessPlanning': return 'fas fa-clipboard-list';
            case 'BusinessOperations': return 'fas fa-chart-line';
            default: return 'fas fa-lightbulb';
        }
    },
    
    /**
     * Gets display name for hub
     */
    getHubDisplayName: function(hub) {
        switch(hub) {
            case 'IdeaDevelopment': return 'Idea Development';
            case 'BusinessPlanning': return 'Business Planning';
            case 'BusinessOperations': return 'Business Operations';
            default: return 'Idea Development';
        }
    },
    
    /**
     * Updates browser state for hub changes
     */
    updateBrowserState: function(hub) {
        // Update page title
        const hubName = this.getHubDisplayName(hub);
        document.title = `${hubName} - IdeaCoach Pro`;
        
        // Store hub preference
        localStorage.setItem('ideacoach_current_hub', hub);
    },
    
    /**
     * Initializes hub router on page load
     */
    initialize: function() {
        console.log('Initializing Hub Router');
        
        // Apply initial hub styling
        const savedHub = localStorage.getItem('ideacoach_current_hub') || 'IdeaDevelopment';
        this.applyHubStyling(savedHub);
        this.currentHub = savedHub;
        
        // Add global styles for transitions
        this.addTransitionStyles();
        
        console.log('Hub Router initialized with hub:', savedHub);
    },
    
    /**
     * Adds global CSS for hub transitions
     */
    addTransitionStyles: function() {
        const style = document.createElement('style');
        style.textContent = `
            .hub-transition-overlay {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background: rgba(0, 0, 0, 0.8);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 9999;
                animation: fadeIn 0.3s ease;
            }
            
            .hub-transition-overlay.fade-out {
                animation: fadeOut 0.3s ease forwards;
            }
            
            .transition-content {
                text-align: center;
                color: white;
                max-width: 400px;
                padding: 2rem;
            }
            
            .transition-icon {
                font-size: 3rem;
                margin-bottom: 1rem;
                color: var(--hub-primary, #7b8fef);
            }
            
            .transition-text h3 {
                margin: 0 0 0.5rem 0;
                font-size: 1.5rem;
                font-weight: 600;
            }
            
            .transition-text p {
                margin: 0 0 1.5rem 0;
                opacity: 0.8;
            }
            
            .transition-progress {
                width: 100%;
                height: 4px;
                background: rgba(255, 255, 255, 0.3);
                border-radius: 2px;
                overflow: hidden;
            }
            
            .progress-bar {
                height: 100%;
                background: var(--hub-primary, #7b8fef);
                width: 0%;
                transition: width 0.6s ease;
            }
            
            .hub-transitioning {
                overflow: hidden;
            }
            
            @keyframes fadeIn {
                from { opacity: 0; }
                to { opacity: 1; }
            }
            
            @keyframes fadeOut {
                from { opacity: 1; }
                to { opacity: 0; }
            }
        `;
        document.head.appendChild(style);
    }
};

// Initialize on DOM load
document.addEventListener('DOMContentLoaded', function() {
    window.hubRouter.initialize();
});

// Initialize on Blazor reconnect
window.addEventListener('beforeunload', function() {
    // Save current state before page unload
    localStorage.setItem('ideacoach_current_hub', window.hubRouter.currentHub);
});