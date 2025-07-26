// Dashboard-specific JavaScript functionality
// Optimized for long-term engagement and business management

// Dashboard state management
let dashboardState = {
    currentPeriod: '30d',
    currentBusiness: 'fitment',
    metrics: {
        revenue: 12450,
        customers: 1247,
        growth: 34.2,
        conversion: 4.2
    },
    revenueShare: {
        total: 12450,
        userShare: 0.85,
        buildlabShare: 0.15
    }
};

// Initialize dashboard when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeDashboard();
});

function initializeDashboard() {
    // Initialize real-time updates
    initializeRealTimeUpdates();
    
    // Initialize interactive elements
    initializePeriodSelector();
    initializeSidebarNavigation();
    initializeMetricCards();
    initializeAICoach();
    initializeRevenueShare();
    
    // Start periodic data updates
    startDataUpdates();
    
    // Initialize engagement tracking
    initializeEngagementTracking();
}

// Real-time updates for live dashboard feel
function initializeRealTimeUpdates() {
    // Simulate real-time revenue updates
    setInterval(() => {
        updateRevenueCounter();
        updateActivityFeed();
    }, 30000); // Update every 30 seconds
    
    // Animate counters on page load
    animateCounters();
}

function updateRevenueCounter() {
    const revenueElement = document.querySelector('.metric-card.revenue .metric-value');
    if (revenueElement) {
        // Simulate small revenue increases
        const currentRevenue = dashboardState.metrics.revenue;
        const increase = Math.floor(Math.random() * 50) + 10; // $10-60 increase
        dashboardState.metrics.revenue += increase;
        
        // Animate the counter update
        animateValueChange(revenueElement, currentRevenue, dashboardState.metrics.revenue);
        
        // Update revenue share calculations
        updateRevenueShareDisplay();
        
        // Show notification for significant increases
        if (increase > 40) {
            showMiniNotification(`💰 +$${increase} revenue just now!`, 'success');
        }
    }
}

function animateCounters() {
    const counters = document.querySelectorAll('.metric-value');
    
    counters.forEach(counter => {
        const target = parseFloat(counter.textContent.replace(/[^0-9.]/g, ''));
        const duration = 2000; // 2 seconds
        const increment = target / (duration / 16); // 60fps
        let current = 0;
        
        const timer = setInterval(() => {
            current += increment;
            if (current >= target) {
                current = target;
                clearInterval(timer);
            }
            
            // Format based on the original text
            if (counter.textContent.includes('$')) {
                counter.textContent = `$${Math.floor(current).toLocaleString()}`;
            } else if (counter.textContent.includes('%')) {
                counter.textContent = `${current.toFixed(1)}%`;
            } else {
                counter.textContent = Math.floor(current).toLocaleString();
            }
        }, 16);
    });
}

function animateValueChange(element, oldValue, newValue) {
    const duration = 1000;
    const increment = (newValue - oldValue) / (duration / 16);
    let current = oldValue;
    
    const timer = setInterval(() => {
        current += increment;
        if (current >= newValue) {
            current = newValue;
            clearInterval(timer);
        }
        element.textContent = `$${Math.floor(current).toLocaleString()}`;
    }, 16);
}

// Period selector functionality
function initializePeriodSelector() {
    const periodButtons = document.querySelectorAll('.period-btn');
    
    periodButtons.forEach(btn => {
        btn.addEventListener('click', () => {
            // Remove active class from all buttons
            periodButtons.forEach(b => b.classList.remove('active'));
            
            // Add active class to clicked button
            btn.classList.add('active');
            
            // Update dashboard data
            const period = btn.dataset.period;
            updateDashboardPeriod(period);
        });
    });
}

function updateDashboardPeriod(period) {
    dashboardState.currentPeriod = period;
    
    // Show loading state
    showLoadingState('Updating metrics...');
    
    // Simulate API call delay
    setTimeout(() => {
        // Update metrics based on period
        updateMetricsForPeriod(period);
        hideLoadingState();
        showToast(`Updated to ${period} view`, 'info');
    }, 800);
}

function updateMetricsForPeriod(period) {
    // Simulate different metrics for different periods
    const periodMultipliers = {
        '7d': { revenue: 0.25, customers: 0.15, growth: 1.2 },
        '30d': { revenue: 1, customers: 1, growth: 1 },
        '90d': { revenue: 2.8, customers: 2.5, growth: 0.8 },
        '1y': { revenue: 12, customers: 10, growth: 0.6 }
    };
    
    const multiplier = periodMultipliers[period];
    
    // Update metric displays
    updateMetricValue('.metric-card.revenue .metric-value', 
        Math.floor(12450 * multiplier.revenue));
    updateMetricValue('.metric-card.customers .metric-value', 
        Math.floor(1247 * multiplier.customers));
    updateMetricValue('.metric-card.growth .metric-value', 
        (34.2 * multiplier.growth).toFixed(1) + '%');
}

function updateMetricValue(selector, value) {
    const element = document.querySelector(selector);
    if (element) {
        if (typeof value === 'string' && value.includes('%')) {
            element.textContent = value;
        } else {
            element.textContent = `$${value.toLocaleString()}`;
        }
    }
}

// Sidebar navigation
function initializeSidebarNavigation() {
    const menuItems = document.querySelectorAll('.menu-item');
    
    menuItems.forEach(item => {
        item.addEventListener('click', () => {
            // Remove active class from all items
            menuItems.forEach(i => i.classList.remove('active'));
            
            // Add active class to clicked item
            item.classList.add('active');
            
            // Handle navigation
            const menuText = item.querySelector('span').textContent;
            handleMenuNavigation(menuText);
        });
    });
}

function handleMenuNavigation(menuItem) {
    // In a real app, this would load different views
    switch(menuItem) {
        case 'Dashboard':
            showToast('Dashboard view loaded', 'info');
            break;
        case 'Goals & Metrics':
            showToast('Goals & Metrics would load here', 'info');
            break;
        case 'Action Items':
            showToast('Action Items view would load here', 'info');
            break;
        case 'Customers':
            showToast('Customer management would load here', 'info');
            break;
        case 'Revenue Share':
            openRevenueShareModal();
            break;
        case 'AI Coach':
            openAICoach();
            break;
        default:
            showToast(`${menuItem} section would load here`, 'info');
    }
}

// Metric cards interactivity
function initializeMetricCards() {
    const metricCards = document.querySelectorAll('.metric-card');
    
    metricCards.forEach(card => {
        // Add hover effects
        card.addEventListener('mouseenter', () => {
            card.style.transform = 'translateY(-2px)';
            card.style.boxShadow = '0 8px 25px rgba(0,0,0,0.1)';
        });
        
        card.addEventListener('mouseleave', () => {
            card.style.transform = 'translateY(0)';
            card.style.boxShadow = '';
        });
        
        // Add click handlers
        card.addEventListener('click', () => {
            const cardType = card.classList[1]; // revenue, customers, etc.
            handleMetricCardClick(cardType);
        });
    });
}

function handleMetricCardClick(cardType) {
    switch(cardType) {
        case 'revenue':
            showToast('Revenue analytics would open here', 'info');
            break;
        case 'customers':
            showToast('Customer analytics would open here', 'info');
            break;
        case 'growth':
            showToast('Growth analytics would open here', 'info');
            break;
        case 'conversion':
            showToast('Conversion analytics would open here', 'info');
            break;
    }
}

// AI Coach functionality
function initializeAICoach() {
    // Initialize chat input
    const chatInput = document.getElementById('chatInput');
    if (chatInput) {
        chatInput.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                sendMessage();
            }
        });
    }
    
    // Initialize suggestion interactions
    initializeCoachSuggestions();
}

function initializeCoachSuggestions() {
    const suggestions = document.querySelectorAll('.suggestion-item');
    
    suggestions.forEach(suggestion => {
        const actionBtn = suggestion.querySelector('.suggestion-action');
        if (actionBtn) {
            actionBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                const suggestionTitle = suggestion.querySelector('h4').textContent;
                handleSuggestionAction(suggestionTitle);
            });
        }
    });
}

function handleSuggestionAction(suggestionTitle) {
    switch(suggestionTitle) {
        case 'Optimize Email Campaign':
            showToast('Opening email campaign optimizer...', 'info');
            break;
        case 'Upsell Opportunity':
            showToast('Loading upsell prospects...', 'info');
            break;
        case 'Mobile Experience':
            showToast('Starting mobile analysis...', 'info');
            break;
    }
}

function openAICoach() {
    const modal = document.getElementById('aiCoachModal');
    if (modal) {
        modal.style.display = 'flex';
        setTimeout(() => {
            modal.classList.add('active');
        }, 10);
        
        // Focus on input
        const chatInput = document.getElementById('chatInput');
        if (chatInput) {
            setTimeout(() => chatInput.focus(), 100);
        }
    }
}

function closeAICoach() {
    const modal = document.getElementById('aiCoachModal');
    if (modal) {
        modal.classList.remove('active');
        setTimeout(() => {
            modal.style.display = 'none';
        }, 300);
    }
}

function sendMessage() {
    const input = document.getElementById('chatInput');
    const message = input.value.trim();
    
    if (message) {
        addUserMessage(message);
        input.value = '';
        
        // Simulate AI response
        setTimeout(() => {
            generateAIResponse(message);
        }, 1000);
    }
}

function addUserMessage(message) {
    const chatMessages = document.getElementById('chatMessages');
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message user-message';
    messageDiv.innerHTML = `
        <div class="message-content">
            <p>${message}</p>
        </div>
    `;
    chatMessages.appendChild(messageDiv);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

function generateAIResponse(userMessage) {
    const responses = {
        'revenue optimization': 'Based on your metrics, I recommend focusing on customer lifetime value. Your current CLV is $890. We can improve this by implementing a loyalty program and upselling premium features.',
        'customer retention': 'Your churn rate of 2.1% is excellent! To maintain this, consider implementing predictive analytics to identify at-risk customers early.',
        'growth strategy': 'Your 34.2% growth rate is phenomenal! To sustain this, I suggest expanding your marketing channels and exploring strategic partnerships.',
        'default': 'I understand you\'re asking about ' + userMessage + '. Let me analyze your business data and provide specific recommendations...'
    };
    
    const responseKey = Object.keys(responses).find(key => 
        userMessage.toLowerCase().includes(key.toLowerCase())
    ) || 'default';
    
    const response = responses[responseKey];
    
    const chatMessages = document.getElementById('chatMessages');
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message coach-message';
    messageDiv.innerHTML = `
        <div class="message-avatar">🤖</div>
        <div class="message-content">
            <p>${response}</p>
        </div>
    `;
    chatMessages.appendChild(messageDiv);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

function askCoach(topic) {
    const input = document.getElementById('chatInput');
    input.value = topic;
    sendMessage();
}

// Revenue share functionality
function initializeRevenueShare() {
    updateRevenueShareDisplay();
    
    // Add click handler to revenue share indicator
    const shareIndicator = document.querySelector('.revenue-share-indicator');
    if (shareIndicator) {
        shareIndicator.addEventListener('click', openRevenueShareModal);
    }
}

function updateRevenueShareDisplay() {
    const totalRevenue = dashboardState.metrics.revenue;
    const userShare = totalRevenue * dashboardState.revenueShare.userShare;
    const buildlabShare = totalRevenue * dashboardState.revenueShare.buildlabShare;
    
    // Update nav indicator
    const shareAmount = document.querySelector('.share-amount');
    if (shareAmount) {
        shareAmount.textContent = `$${Math.floor(buildlabShare).toLocaleString()} this month`;
    }
    
    // Update revenue card breakdown
    const yourShareElement = document.querySelector('.split-item.yours .split-value');
    const buildlabShareElement = document.querySelector('.split-item.buildlab .split-value');
    
    if (yourShareElement) {
        yourShareElement.textContent = `$${Math.floor(userShare).toLocaleString()}`;
    }
    
    if (buildlabShareElement) {
        buildlabShareElement.textContent = `$${Math.floor(buildlabShare).toLocaleString()}`;
    }
}

function openRevenueShareModal() {
    const modal = document.getElementById('revenueShareModal');
    if (modal) {
        modal.style.display = 'flex';
        setTimeout(() => {
            modal.classList.add('active');
        }, 10);
    }
}

function closeRevenueShareModal() {
    const modal = document.getElementById('revenueShareModal');
    if (modal) {
        modal.classList.remove('active');
        setTimeout(() => {
            modal.style.display = 'none';
        }, 300);
    }
}

// Activity feed updates
function updateActivityFeed() {
    const activities = [
        { icon: 'fas fa-dollar-sign', type: 'revenue', text: `$${Math.floor(Math.random() * 500) + 100} payment received from ${getRandomName()}`, time: 'Just now' },
        { icon: 'fas fa-user-plus', type: 'customer', text: `New customer signup: ${getRandomName()}`, time: `${Math.floor(Math.random() * 30) + 1} minutes ago` },
        { icon: 'fas fa-star', type: 'review', text: `5-star review received from ${getRandomName()}`, time: `${Math.floor(Math.random() * 2) + 1} hour ago` }
    ];
    
    const randomActivity = activities[Math.floor(Math.random() * activities.length)];
    addActivityItem(randomActivity);
}

function addActivityItem(activity) {
    const activityList = document.querySelector('.activity-list');
    if (activityList) {
        // Remove oldest item if more than 5
        const items = activityList.querySelectorAll('.activity-item');
        if (items.length >= 5) {
            items[items.length - 1].remove();
        }
        
        // Add new item at top
        const newItem = document.createElement('div');
        newItem.className = 'activity-item';
        newItem.innerHTML = `
            <div class="activity-icon ${activity.type}">
                <i class="${activity.icon}"></i>
            </div>
            <div class="activity-content">
                <div class="activity-text">
                    <strong>${activity.text}</strong>
                </div>
                <div class="activity-time">${activity.time}</div>
            </div>
        `;
        
        activityList.insertBefore(newItem, activityList.firstChild);
        
        // Add fade-in animation
        newItem.style.opacity = '0';
        newItem.style.transform = 'translateY(-10px)';
        setTimeout(() => {
            newItem.style.transition = 'all 0.3s ease';
            newItem.style.opacity = '1';
            newItem.style.transform = 'translateY(0)';
        }, 10);
    }
}

function getRandomName() {
    const names = ['Sarah M.', 'Michael D.', 'Lisa K.', 'David W.', 'Emma R.', 'John S.', 'Maria G.', 'Alex T.'];
    return names[Math.floor(Math.random() * names.length)];
}

// Quick actions
function createCampaign() {
    showToast('Campaign creator would open here', 'info');
}

function addProduct() {
    showToast('Product management would open here', 'info');
}

function sendEmail() {
    showToast('Email composer would open here', 'info');
}

function viewAnalytics() {
    showToast('Advanced analytics would open here', 'info');
}

function manageTeam() {
    showToast('Team management would open here', 'info');
}

function openSupport() {
    showToast('Support center would open here', 'info');
}

// Data updates simulation
function startDataUpdates() {
    // Simulate periodic metric updates
    setInterval(() => {
        updateHealthScore();
        updateGrowthMetrics();
    }, 60000); // Update every minute
}

function updateHealthScore() {
    const scoreNumber = document.querySelector('.score-number');
    if (scoreNumber) {
        const currentScore = parseInt(scoreNumber.textContent);
        const variation = Math.floor(Math.random() * 3) - 1; // -1, 0, or 1
        const newScore = Math.max(70, Math.min(100, currentScore + variation));
        
        if (newScore !== currentScore) {
            animateValueChange(scoreNumber, currentScore, newScore);
        }
    }
}

function updateGrowthMetrics() {
    // Simulate small changes in growth metrics
    const growthCard = document.querySelector('.metric-card.growth .metric-value');
    if (growthCard) {
        const currentGrowth = parseFloat(growthCard.textContent);
        const variation = (Math.random() - 0.5) * 2; // -1 to 1
        const newGrowth = Math.max(0, currentGrowth + variation);
        
        growthCard.textContent = `${newGrowth.toFixed(1)}%`;
    }
}

// Engagement tracking for UX optimization
function initializeEngagementTracking() {
    // Track time spent on dashboard
    let startTime = Date.now();
    let isActive = true;
    
    // Track user activity
    document.addEventListener('click', logUserInteraction);
    document.addEventListener('scroll', logUserInteraction);
    document.addEventListener('keypress', logUserInteraction);
    
    // Track page visibility
    document.addEventListener('visibilitychange', () => {
        if (document.hidden) {
            isActive = false;
            logSessionData(Date.now() - startTime);
        } else {
            isActive = true;
            startTime = Date.now();
        }
    });
    
    // Log engagement metrics every 5 minutes
    setInterval(() => {
        if (isActive) {
            logEngagementMetrics();
        }
    }, 300000);
}

function logUserInteraction(event) {
    // In a real app, this would send data to analytics
    console.log('User interaction:', event.type, event.target);
}

function logSessionData(duration) {
    // In a real app, this would send session data to analytics
    console.log('Session duration:', duration, 'ms');
}

function logEngagementMetrics() {
    // In a real app, this would send engagement data to analytics
    console.log('User engaged for 5+ minutes');
}

// Mini notification system for dashboard
function showMiniNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `mini-notification ${type}`;
    notification.innerHTML = `
        <span>${message}</span>
        <button onclick="this.parentElement.remove()">
            <i class="fas fa-times"></i>
        </button>
    `;
    
    // Add styles if not present
    if (!document.getElementById('miniNotificationStyles')) {
        const styles = document.createElement('style');
        styles.id = 'miniNotificationStyles';
        styles.textContent = `
            .mini-notification {
                position: fixed;
                top: 80px;
                right: 20px;
                background: white;
                padding: 12px 16px;
                border-radius: 6px;
                box-shadow: 0 4px 12px rgba(0,0,0,0.1);
                z-index: 1000;
                transform: translateX(300px);
                transition: transform 0.3s ease;
                font-size: 14px;
                display: flex;
                align-items: center;
                gap: 8px;
            }
            .mini-notification.show {
                transform: translateX(0);
            }
            .mini-notification.success {
                border-left: 3px solid var(--success-color);
            }
            .mini-notification button {
                background: none;
                border: none;
                color: #666;
                cursor: pointer;
                padding: 0;
                width: 16px;
                height: 16px;
            }
        `;
        document.head.appendChild(styles);
    }
    
    document.body.appendChild(notification);
    
    setTimeout(() => notification.classList.add('show'), 10);
    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
    }, 4000);
}

// Export functions for global use
window.openAICoach = openAICoach;
window.closeAICoach = closeAICoach;
window.sendMessage = sendMessage;
window.askCoach = askCoach;
window.openRevenueShareModal = openRevenueShareModal;
window.closeRevenueShareModal = closeRevenueShareModal;
window.createCampaign = createCampaign;
window.addProduct = addProduct;
window.sendEmail = sendEmail;
window.viewAnalytics = viewAnalytics;
window.manageTeam = manageTeam;
window.openSupport = openSupport;