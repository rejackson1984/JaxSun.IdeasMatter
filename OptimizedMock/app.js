// BuildLab JavaScript Application
// Main functionality for all pages

// Global variables
let currentUser = null;
let isLoggedIn = false;

// DOM Content Loaded Event
document.addEventListener('DOMContentLoaded', function() {
    initializeApp();
});

// Initialize Application
function initializeApp() {
    // Initialize mobile menu
    initializeMobileMenu();
    
    // Initialize modals
    initializeModals();
    
    // Initialize pricing toggle
    initializePricingToggle();
    
    // Initialize FAQ
    initializeFAQ();
    
    // Initialize ROI calculator
    initializeROICalculator();
    
    // Initialize scroll effects
    initializeScrollEffects();
    
    // Initialize animations
    initializeAnimations();
    
    // Check authentication state
    checkAuthState();
}

// Mobile Menu Functionality
function initializeMobileMenu() {
    const mobileToggle = document.getElementById('mobile-toggle');
    const navMenu = document.getElementById('nav-menu');
    
    if (mobileToggle && navMenu) {
        mobileToggle.addEventListener('click', function() {
            mobileToggle.classList.toggle('active');
            navMenu.classList.toggle('active');
        });
        
        // Close mobile menu when clicking outside
        document.addEventListener('click', function(e) {
            if (!mobileToggle.contains(e.target) && !navMenu.contains(e.target)) {
                mobileToggle.classList.remove('active');
                navMenu.classList.remove('active');
            }
        });
    }
}

// Modal Functionality
function initializeModals() {
    // Close modal when clicking outside
    document.addEventListener('click', function(e) {
        if (e.target.classList.contains('modal-overlay')) {
            closeAllModals();
        }
    });
    
    // Close modal on escape key
    document.addEventListener('keydown', function(e) {
        if (e.key === 'Escape') {
            closeAllModals();
        }
    });
}

// Login Modal Functions
function showLoginModal() {
    const modal = document.getElementById('loginModal');
    if (modal) {
        modal.style.display = 'flex';
        setTimeout(() => {
            modal.classList.add('active');
        }, 10);
    }
}

function closeLoginModal() {
    const modal = document.getElementById('loginModal');
    if (modal) {
        modal.classList.remove('active');
        setTimeout(() => {
            modal.style.display = 'none';
        }, 300);
    }
}

function closeAllModals() {
    const modals = document.querySelectorAll('.modal-overlay');
    modals.forEach(modal => {
        modal.classList.remove('active');
        setTimeout(() => {
            modal.style.display = 'none';
        }, 300);
    });
}

// Authentication Functions
function loginUser(event) {
    if (event) {
        event.preventDefault();
    }
    
    const email = document.getElementById('loginEmail')?.value;
    const password = document.getElementById('loginPassword')?.value;
    
    // Simulate login process
    if (email && password) {
        showLoadingState('Signing you in...');
        
        setTimeout(() => {
            currentUser = {
                email: email,
                name: getNameFromEmail(email),
                plan: 'professional'
            };
            isLoggedIn = true;
            
            // Store in localStorage
            localStorage.setItem('buildlab_user', JSON.stringify(currentUser));
            localStorage.setItem('buildlab_logged_in', 'true');
            
            hideLoadingState();
            closeLoginModal();
            showSuccessMessage('Welcome back! Redirecting to your dashboard...');
            
            // Redirect to dashboard after short delay
            setTimeout(() => {
                window.location.href = 'dashboard.html';
            }, 1500);
        }, 2000);
    } else {
        showToast('Please enter your email and password', 'error');
    }
}

function startOnboarding() {
    showLoadingState('Starting your journey...');
    
    setTimeout(() => {
        hideLoadingState();
        showSuccessMessage('Let\'s get started! Creating your account...');
        
        // Simulate onboarding process
        setTimeout(() => {
            // Create demo user and redirect to dashboard
            currentUser = {
                email: 'demo@buildlab.com',
                name: 'Demo User',
                plan: 'professional'
            };
            isLoggedIn = true;
            
            localStorage.setItem('buildlab_user', JSON.stringify(currentUser));
            localStorage.setItem('buildlab_logged_in', 'true');
            
            window.location.href = 'dashboard.html';
        }, 1500);
    }, 1500);
}

function checkAuthState() {
    const storedUser = localStorage.getItem('buildlab_user');
    const storedLoginState = localStorage.getItem('buildlab_logged_in');
    
    if (storedUser && storedLoginState === 'true') {
        currentUser = JSON.parse(storedUser);
        isLoggedIn = true;
        updateUIForLoggedInUser();
    }
}

function updateUIForLoggedInUser() {
    // Update navigation
    const loginBtn = document.querySelector('.btn-nav-login');
    const signupBtn = document.querySelector('.btn-nav-signup');
    
    if (loginBtn && currentUser) {
        loginBtn.textContent = `Hi, ${currentUser.name}`;
        loginBtn.onclick = () => window.location.href = 'dashboard.html';
    }
    
    if (signupBtn) {
        signupBtn.textContent = 'Dashboard';
        signupBtn.onclick = () => window.location.href = 'dashboard.html';
    }
}

// Pricing Toggle Functionality
function initializePricingToggle() {
    const toggle = document.getElementById('billingToggle');
    
    if (toggle) {
        toggle.addEventListener('change', function() {
            togglePricingBilling(this.checked);
        });
    }
}

function togglePricingBilling(isAnnual) {
    const monthlyPrices = document.querySelectorAll('.price-amount.monthly, .price-note.monthly');
    const annualPrices = document.querySelectorAll('.price-amount.annual, .price-note.annual');
    const toggleLabels = document.querySelectorAll('.toggle-label');
    
    if (isAnnual) {
        monthlyPrices.forEach(el => el.style.display = 'none');
        annualPrices.forEach(el => el.style.display = 'inline');
        toggleLabels[0]?.classList.remove('active');
        toggleLabels[1]?.classList.add('active');
    } else {
        monthlyPrices.forEach(el => el.style.display = 'inline');
        annualPrices.forEach(el => el.style.display = 'none');
        toggleLabels[0]?.classList.add('active');
        toggleLabels[1]?.classList.remove('active');
    }
    
    // Update ROI calculator if present
    updateROICalculator();
}

// Plan Selection
function selectPlan(planType) {
    const plans = {
        starter: {
            name: 'Starter',
            price: 0,
            description: 'Perfect for validating your first idea'
        },
        professional: {
            name: 'Professional', 
            price: 97,
            description: 'For serious entrepreneurs ready to launch'
        },
        enterprise: {
            name: 'Enterprise',
            price: 297,
            description: 'For teams and scaling businesses'
        }
    };
    
    const plan = plans[planType];
    
    if (planType === 'starter') {
        showSuccessMessage(`Starting with ${plan.name} plan. Let's validate your idea!`);
        setTimeout(() => startOnboarding(), 1500);
    } else if (planType === 'enterprise') {
        showContactModal(plan);
    } else {
        showTrialModal(plan);
    }
}

function showTrialModal(plan) {
    showSuccessMessage(`Starting your 14-day free trial of ${plan.name}. No charges until trial ends!`);
    setTimeout(() => startOnboarding(), 1500);
}

function showContactModal(plan) {
    showSuccessMessage(`Thanks for your interest in ${plan.name}! Our sales team will contact you within 2 hours.`);
}

// FAQ Functionality
function initializeFAQ() {
    const faqItems = document.querySelectorAll('.faq-item');
    
    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');
        const answer = item.querySelector('.faq-answer');
        const icon = question?.querySelector('i');
        
        if (question && answer) {
            question.addEventListener('click', () => {
                const isOpen = item.classList.contains('open');
                
                // Close all other FAQ items
                faqItems.forEach(otherItem => {
                    if (otherItem !== item) {
                        otherItem.classList.remove('open');
                        const otherAnswer = otherItem.querySelector('.faq-answer');
                        const otherIcon = otherItem.querySelector('.faq-question i');
                        if (otherAnswer) otherAnswer.style.maxHeight = '0';
                        if (otherIcon) otherIcon.style.transform = 'rotate(0deg)';
                    }
                });
                
                // Toggle current item
                if (!isOpen) {
                    item.classList.add('open');
                    answer.style.maxHeight = answer.scrollHeight + 'px';
                    if (icon) icon.style.transform = 'rotate(180deg)';
                } else {
                    item.classList.remove('open');
                    answer.style.maxHeight = '0';
                    if (icon) icon.style.transform = 'rotate(0deg)';
                }
            });
        }
    });
}

// ROI Calculator
function initializeROICalculator() {
    const inputs = ['hourlyRate', 'hoursPerWeek', 'currentStage'];
    
    inputs.forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener('input', updateROICalculator);
            element.addEventListener('change', updateROICalculator);
        }
    });
    
    // Initial calculation
    updateROICalculator();
}

function updateROICalculator() {
    const hourlyRate = parseFloat(document.getElementById('hourlyRate')?.value) || 50;
    const hoursPerWeek = parseFloat(document.getElementById('hoursPerWeek')?.value) || 20;
    const currentStage = document.getElementById('currentStage')?.value || 'idea';
    
    // Calculate based on 40% time savings and revenue multipliers
    const timeSavedPerWeek = hoursPerWeek * 0.4;
    const timeSavedPerMonth = timeSavedPerWeek * 4.33;
    const valueSaved = timeSavedPerMonth * hourlyRate;
    
    // Revenue potential based on stage
    const revenueMultipliers = {
        idea: 300,
        planning: 400,
        building: 500,
        launching: 600
    };
    
    const potentialRevenue = (revenueMultipliers[currentStage] || 300) * (hourlyRate / 10);
    const planCost = 97; // Professional plan
    const roi = ((valueSaved + potentialRevenue - planCost) / planCost * 100);
    
    // Update display
    updateElementText('timeSaved', `${timeSavedPerWeek.toFixed(1)} hours`);
    updateElementText('valueSaved', `$${valueSaved.toLocaleString()}`);
    updateElementText('revenueGain', `$${potentialRevenue.toLocaleString()}`);
    updateElementText('totalROI', `${roi.toFixed(0)}%`);
}

// Scroll Effects
function initializeScrollEffects() {
    const navbar = document.getElementById('navbar');
    
    if (navbar) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 100) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }
    
    // Parallax effect for hero backgrounds
    const heroSections = document.querySelectorAll('.hero, .hero-how, .hero-stories, .hero-pricing');
    
    window.addEventListener('scroll', () => {
        const scrolled = window.pageYOffset;
        
        heroSections.forEach(hero => {
            const rate = scrolled * -0.5;
            const background = hero.querySelector('.hero-background');
            if (background) {
                background.style.transform = `translateY(${rate}px)`;
            }
        });
    });
}

// Animations
function initializeAnimations() {
    // Intersection Observer for fade-in animations
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animateElements = document.querySelectorAll('.story-card, .step-section, .metric-card, .feature-card, .pricing-card');
    animateElements.forEach(el => {
        observer.observe(el);
    });
}

// Story Detail Functions
function showStoryDetail(storyId) {
    const stories = {
        jessica: {
            name: 'Jessica Chen',
            business: 'FitMind',
            details: 'Complete journey from barista to $2.1M app founder...'
        },
        marcus: {
            name: 'Marcus Johnson', 
            business: 'EcoPackaging',
            details: 'College student who built a $5M sustainable packaging company...'
        },
        // Add more stories as needed
    };
    
    const story = stories[storyId];
    if (story) {
        showToast(`${story.name}'s full story would open here in the complete app`, 'info');
    }
}

// Video Functions
function playVideo(videoId) {
    showToast(`${videoId} video would play here in the complete app`, 'info');
}

// Utility Functions
function showLoadingState(message = 'Loading...') {
    // Create loading overlay
    const overlay = document.createElement('div');
    overlay.id = 'loadingOverlay';
    overlay.className = 'loading-overlay';
    overlay.innerHTML = `
        <div class="loading-content">
            <div class="loading-spinner"></div>
            <p>${message}</p>
        </div>
    `;
    
    // Add styles
    const styles = document.createElement('style');
    styles.textContent = `
        .loading-overlay {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background: rgba(0, 0, 0, 0.8);
            display: flex;
            align-items: center;
            justify-content: center;
            z-index: 10000;
        }
        .loading-content {
            text-align: center;
            color: white;
        }
        .loading-spinner {
            width: 40px;
            height: 40px;
            border: 3px solid rgba(255, 255, 255, 0.3);
            border-top: 3px solid var(--primary-color);
            border-radius: 50%;
            animation: spin 1s linear infinite;
            margin: 0 auto 16px;
        }
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
    `;
    
    document.head.appendChild(styles);
    document.body.appendChild(overlay);
}

function hideLoadingState() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.remove();
    }
}

function showSuccessMessage(message) {
    showToast(message, 'success');
}

function showToast(message, type = 'info') {
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <div class="toast-content">
            <i class="fas fa-${getToastIcon(type)}"></i>
            <span>${message}</span>
        </div>
    `;
    
    // Add styles if not already present
    if (!document.getElementById('toastStyles')) {
        const styles = document.createElement('style');
        styles.id = 'toastStyles';
        styles.textContent = `
            .toast {
                position: fixed;
                top: 20px;
                right: 20px;
                background: white;
                padding: 16px 20px;
                border-radius: 8px;
                box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
                z-index: 10001;
                transform: translateX(400px);
                transition: transform 0.3s ease;
                max-width: 400px;
            }
            .toast.show {
                transform: translateX(0);
            }
            .toast-content {
                display: flex;
                align-items: center;
                gap: 12px;
            }
            .toast-success {
                border-left: 4px solid var(--success-color);
            }
            .toast-error {
                border-left: 4px solid var(--error-color);
            }
            .toast-info {
                border-left: 4px solid var(--primary-color);
            }
        `;
        document.head.appendChild(styles);
    }
    
    document.body.appendChild(toast);
    
    // Trigger animation
    setTimeout(() => toast.classList.add('show'), 100);
    
    // Auto remove
    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 5000);
}

function getToastIcon(type) {
    const icons = {
        success: 'check-circle',
        error: 'exclamation-circle',
        info: 'info-circle'
    };
    return icons[type] || 'info-circle';
}

function updateElementText(id, text) {
    const element = document.getElementById(id);
    if (element) {
        element.textContent = text;
    }
}

function getNameFromEmail(email) {
    const name = email.split('@')[0];
    return name.charAt(0).toUpperCase() + name.slice(1);
}

// Export functions for global use
window.showLoginModal = showLoginModal;
window.closeLoginModal = closeLoginModal;
window.loginUser = loginUser;
window.startOnboarding = startOnboarding;
window.selectPlan = selectPlan;
window.showStoryDetail = showStoryDetail;
window.playVideo = playVideo;