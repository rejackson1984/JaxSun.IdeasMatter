// Dashboard JavaScript for IdeaCoach Pro

// Tooltip System
class TooltipManager {
    constructor() {
        this.tooltip = null;
        this.init();
    }

    init() {
        // Create tooltip element
        this.tooltip = document.getElementById('tooltip');
        if (!this.tooltip) {
            this.tooltip = document.createElement('div');
            this.tooltip.id = 'tooltip';
            this.tooltip.className = 'tooltip';
            this.tooltip.innerHTML = `
                <div class="tooltip-arrow"></div>
                <div class="tooltip-content"></div>
            `;
            document.body.appendChild(this.tooltip);
        }

        // Bind events to all elements with data-tooltip
        this.bindTooltips();
    }

    bindTooltips() {
        const elements = document.querySelectorAll('[data-tooltip]');
        elements.forEach(element => {
            element.addEventListener('mouseenter', (e) => this.showTooltip(e));
            element.addEventListener('mouseleave', () => this.hideTooltip());
            element.addEventListener('mousemove', (e) => this.updateTooltipPosition(e));
        });
    }

    showTooltip(event) {
        const element = event.target.closest('[data-tooltip]');
        if (!element) return;

        const text = element.getAttribute('data-tooltip');
        if (!text) return;

        this.tooltip.querySelector('.tooltip-content').textContent = text;
        this.updateTooltipPosition(event);
        this.tooltip.classList.add('visible');
    }

    hideTooltip() {
        this.tooltip.classList.remove('visible');
        this.tooltip.className = 'tooltip'; // Reset positioning classes
    }

    updateTooltipPosition(event) {
        const rect = this.tooltip.getBoundingClientRect();
        const viewportWidth = window.innerWidth;
        const viewportHeight = window.innerHeight;
        
        let x = event.clientX;
        let y = event.clientY;
        
        // Position tooltip above element by default
        y -= rect.height + 10;
        
        // Adjust horizontally to stay in viewport
        if (x + rect.width / 2 > viewportWidth) {
            x = viewportWidth - rect.width - 10;
        } else if (x - rect.width / 2 < 0) {
            x = 10;
        } else {
            x -= rect.width / 2;
        }
        
        // If tooltip would go above viewport, position below
        if (y < 0) {
            y = event.clientY + 10;
            this.tooltip.classList.add('bottom');
        } else {
            this.tooltip.classList.add('top');
        }
        
        this.tooltip.style.left = x + 'px';
        this.tooltip.style.top = y + 'px';
    }
}

// Progress Animation System
class ProgressAnimator {
    constructor() {
        this.init();
    }

    init() {
        this.animateProgressBars();
        this.animateProgressCircle();
        this.animateProgressRing();
    }

    animateProgressBars() {
        const progressBars = document.querySelectorAll('.progress-bar[style*="width"]');
        progressBars.forEach(bar => {
            const targetWidth = bar.style.width;
            bar.style.width = '0%';
            
            setTimeout(() => {
                bar.style.transition = 'width 1.5s ease-in-out';
                bar.style.width = targetWidth;
            }, 300);
        });
    }

    animateProgressCircle() {
        const progressCircle = document.querySelector('.progress-circle circle[stroke-dashoffset]');
        if (!progressCircle) return;

        const targetOffset = progressCircle.getAttribute('stroke-dashoffset');
        progressCircle.setAttribute('stroke-dashoffset', '157');
        
        setTimeout(() => {
            progressCircle.style.transition = 'stroke-dashoffset 2s ease-in-out';
            progressCircle.setAttribute('stroke-dashoffset', targetOffset);
        }, 500);
    }

    animateProgressRing() {
        const progressRing = document.querySelector('.ring-fill');
        if (!progressRing) return;

        const targetRotation = progressRing.style.transform;
        progressRing.style.transform = 'rotate(0deg)';
        
        setTimeout(() => {
            progressRing.style.transition = 'transform 1.5s ease-in-out';
            progressRing.style.transform = targetRotation;
        }, 700);
    }
}

// Task Management System
class TaskManager {
    constructor() {
        this.init();
    }

    init() {
        this.bindTaskActions();
        this.updateTaskProgress();
    }

    bindTaskActions() {
        const taskButtons = document.querySelectorAll('.task-actions button');
        taskButtons.forEach(button => {
            button.addEventListener('click', (e) => this.handleTaskAction(e));
        });
    }

    handleTaskAction(event) {
        const button = event.target.closest('button');
        const taskItem = button.closest('.task-item');
        const action = button.querySelector('i').classList.contains('fa-play') ? 'continue' : 
                      button.querySelector('i').classList.contains('fa-eye') ? 'view' : 'other';

        switch(action) {
            case 'continue':
                this.continueTask(taskItem);
                break;
            case 'view':
                this.viewTask(taskItem);
                break;
            default:
                this.showTaskOptions(taskItem);
        }
    }

    continueTask(taskItem) {
        // Simulate task continuation
        const progressRing = taskItem.querySelector('.ring-fill');
        if (progressRing) {
            const currentRotation = parseInt(progressRing.style.transform.match(/\d+/)?.[0] || 0);
            const newRotation = Math.min(currentRotation + 36, 324); // Add 10% (36 degrees)
            progressRing.style.transform = `rotate(${newRotation}deg)`;
            
            // Update progress percentage in task meta
            const progressText = taskItem.querySelector('.progress-text');
            if (progressText) {
                const newPercentage = Math.round((newRotation / 324) * 100);
                progressText.textContent = `${newPercentage}% complete`;
            }
        }

        // Show toast notification
        this.showToast('Task progress updated! Keep up the great work!', 'success');
    }

    viewTask(taskItem) {
        const taskTitle = taskItem.querySelector('h4').textContent;
        this.showTaskModal(taskTitle);
    }

    showTaskModal(taskTitle) {
        const modal = document.createElement('div');
        modal.className = 'task-modal';
        modal.innerHTML = `
            <div class="modal-backdrop"></div>
            <div class="modal-content">
                <div class="modal-header">
                    <h3>${taskTitle}</h3>
                    <button class="modal-close" onclick="this.closest('.task-modal').remove()">
                        <i class="fas fa-times"></i>
                    </button>
                </div>
                <div class="modal-body">
                    <div class="task-details">
                        <div class="detail-section">
                            <h4>What you've accomplished:</h4>
                            <ul>
                                <li>Identified target demographics (ages 25-40)</li>
                                <li>Analyzed user pain points and motivations</li>
                                <li>Created detailed user personas</li>
                                <li>Validated assumptions with market data</li>
                            </ul>
                        </div>
                        <div class="detail-section">
                            <h4>Key insights discovered:</h4>
                            <div class="insight-item">
                                <i class="fas fa-lightbulb"></i>
                                <p>73% of fitness app users abandon apps within 3 months due to lack of habit formation features</p>
                            </div>
                            <div class="insight-item">
                                <i class="fas fa-target"></i>
                                <p>Your target audience values community support 3x more than individual tracking</p>
                            </div>
                        </div>
                        <div class="detail-section">
                            <h4>Coach recommendations:</h4>
                            <div class="coach-recommendation">
                                <div class="coach-avatar-small">
                                    <i class="fas fa-robot"></i>
                                </div>
                                <p>"Based on your research, consider adding social features and habit-building gamification to differentiate from competitors like MyFitnessPal."</p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Add styles for task modal
        const style = document.createElement('style');
        style.textContent = `
            .task-modal {
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                z-index: 10001;
                display: flex;
                align-items: center;
                justify-content: center;
            }
            .task-modal .modal-content {
                max-width: 600px;
                max-height: 80vh;
                overflow-y: auto;
            }
            .task-details {
                padding: 20px 0;
            }
            .detail-section {
                margin-bottom: 24px;
            }
            .detail-section h4 {
                color: #1a1a1a;
                margin-bottom: 12px;
                font-weight: 600;
            }
            .detail-section ul {
                list-style: none;
                padding: 0;
            }
            .detail-section li {
                padding: 8px 0;
                padding-left: 24px;
                position: relative;
                color: #64748b;
            }
            .detail-section li::before {
                content: '✓';
                position: absolute;
                left: 0;
                color: #10b981;
                font-weight: bold;
            }
            .insight-item {
                display: flex;
                gap: 12px;
                padding: 16px;
                background: rgba(102, 126, 234, 0.05);
                border-radius: 8px;
                margin-bottom: 12px;
            }
            .insight-item i {
                color: #667eea;
                margin-top: 2px;
            }
            .insight-item p {
                margin: 0;
                color: #1a1a1a;
            }
            .coach-recommendation {
                display: flex;
                gap: 12px;
                padding: 16px;
                background: linear-gradient(135deg, rgba(102, 126, 234, 0.05) 0%, rgba(118, 75, 162, 0.05) 100%);
                border-radius: 8px;
            }
            .coach-avatar-small {
                width: 32px;
                height: 32px;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                color: white;
                font-size: 0.9rem;
                flex-shrink: 0;
            }
            .coach-recommendation p {
                margin: 0;
                color: #1a1a1a;
                font-style: italic;
            }
        `;
        document.head.appendChild(style);
        
        document.body.appendChild(modal);
        
        // Close modal when clicking backdrop
        modal.querySelector('.modal-backdrop').addEventListener('click', () => {
            modal.remove();
        });
    }

    updateTaskProgress() {
        const completedTasks = document.querySelectorAll('.task-item.completed').length;
        const totalTasks = document.querySelectorAll('.task-item').length;
        const progressPercentage = Math.round((completedTasks / totalTasks) * 100);
        
        const taskProgressBar = document.querySelector('.task-progress .progress-bar');
        if (taskProgressBar) {
            setTimeout(() => {
                taskProgressBar.style.width = `${progressPercentage}%`;
            }, 1000);
        }
    }

    showToast(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <div class="toast-content">
                <i class="fas fa-${type === 'success' ? 'check-circle' : 'info-circle'}"></i>
                <span>${message}</span>
            </div>
        `;
        
        // Add toast styles
        const style = document.createElement('style');
        style.textContent = `
            .toast {
                position: fixed;
                top: 100px;
                right: 24px;
                background: white;
                border-radius: 8px;
                padding: 16px;
                box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
                border: 1px solid rgba(0, 0, 0, 0.05);
                z-index: 10000;
                transform: translateX(100%);
                transition: transform 0.3s ease;
            }
            .toast.toast-success {
                border-left: 4px solid #10b981;
            }
            .toast.toast-info {
                border-left: 4px solid #667eea;
            }
            .toast-content {
                display: flex;
                align-items: center;
                gap: 12px;
                color: #1a1a1a;
            }
            .toast-success .toast-content i {
                color: #10b981;
            }
            .toast-info .toast-content i {
                color: #667eea;
            }
            .toast.show {
                transform: translateX(0);
            }
        `;
        document.head.appendChild(style);
        
        document.body.appendChild(toast);
        
        // Animate in
        setTimeout(() => toast.classList.add('show'), 100);
        
        // Remove after 3 seconds
        setTimeout(() => {
            toast.style.transform = 'translateX(100%)';
            setTimeout(() => toast.remove(), 300);
        }, 3000);
    }
}

// Coach Chat System
class CoachChat {
    constructor() {
        this.modal = null;
        this.chatMessages = null;
        this.chatInput = null;
        this.init();
    }

    init() {
        this.modal = document.getElementById('coachModal');
        this.chatMessages = document.getElementById('chatMessages');
        this.chatInput = document.getElementById('chatInput');
        
        this.bindEvents();
        this.setupAutoResponses();
    }

    bindEvents() {
        // Bind coach buttons
        const coachButtons = document.querySelectorAll('.btn-coach, .btn-ghost[data-tooltip*="coach"]');
        coachButtons.forEach(button => {
            button.addEventListener('click', () => this.openChat());
        });
        
        // Bind chat input
        if (this.chatInput) {
            this.chatInput.addEventListener('keypress', (e) => {
                if (e.key === 'Enter') this.sendMessage();
            });
        }
    }

    openChat() {
        if (this.modal) {
            this.modal.classList.add('active');
            this.chatInput?.focus();
        }
    }

    closeChat() {
        if (this.modal) {
            this.modal.classList.remove('active');
        }
    }

    sendMessage() {
        const message = this.chatInput?.value.trim();
        if (!message) return;

        // Add user message
        this.addMessage(message, 'user');
        this.chatInput.value = '';

        // Generate AI response
        setTimeout(() => {
            const response = this.generateResponse(message);
            this.addMessage(response, 'coach');
        }, 1000 + Math.random() * 2000);
    }

    addMessage(content, sender) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${sender}-message`;
        
        if (sender === 'coach') {
            messageDiv.innerHTML = `
                <div class="coach-avatar">
                    <i class="fas fa-robot"></i>
                </div>
                <div class="message-content">
                    <p>${content}</p>
                </div>
            `;
        } else {
            messageDiv.innerHTML = `
                <div class="message-content">
                    <p>${content}</p>
                </div>
            `;
        }
        
        this.chatMessages?.appendChild(messageDiv);
        this.chatMessages?.scrollTo(0, this.chatMessages.scrollHeight);
    }

    generateResponse(userMessage) {
        const responses = {
            'financial': [
                "Great question about finances! Based on your fitness app, I'd recommend starting with a freemium model. Here's why: 73% of successful fitness apps use this approach to build a user base first.",
                "For financial projections, let's focus on your customer acquisition cost (CAC) and lifetime value (LTV). In the fitness app space, a good LTV:CAC ratio is 3:1.",
                "I see you're working on financial projections. Let me help you estimate: with 1,000 users in Year 1, assuming 15% convert to premium at $9.99/month, you're looking at ~$18K monthly recurring revenue."
            ],
            'market': [
                "The fitness app market is worth $4.4B and growing 14% annually. Your timing is perfect! The key is finding your unique positioning.",
                "Based on your market research, I notice you're targeting the habit-formation gap. Smart move - 67% of fitness app users struggle with consistency.",
                "Your competitor analysis shows MyFitnessPal dominates calorie tracking, but there's a huge opportunity in habit coaching and community features."
            ],
            'design': [
                "For your app design, focus on simplicity and habit formation. Users should be able to log a workout in under 30 seconds.",
                "I recommend a progress-driven UI. Show streaks, achievements, and social proof prominently. This increases retention by 45%.",
                "Your design phase should start with user journey mapping. Want me to walk you through creating user flows for your top 3 features?"
            ],
            'general': [
                "I'm here to help you succeed! What specific aspect of your business would you like to focus on today?",
                "Based on your progress, you're doing excellently! You're in the top 20% of entrepreneurs at this stage.",
                "Remember, every successful business started with someone just like you - passionate and willing to learn. What's your biggest challenge right now?",
                "I notice you've completed 18 tasks so far. That's impressive dedication! What would you like to tackle next?",
                "Your validation score of 87% is outstanding. Most successful businesses launch with 70%+ validation. You're ready for the next phase!"
            ]
        };

        const lowerMessage = userMessage.toLowerCase();
        
        if (lowerMessage.includes('financial') || lowerMessage.includes('money') || lowerMessage.includes('revenue') || lowerMessage.includes('cost')) {
            return responses.financial[Math.floor(Math.random() * responses.financial.length)];
        } else if (lowerMessage.includes('market') || lowerMessage.includes('competitor') || lowerMessage.includes('research')) {
            return responses.market[Math.floor(Math.random() * responses.market.length)];
        } else if (lowerMessage.includes('design') || lowerMessage.includes('ui') || lowerMessage.includes('ux') || lowerMessage.includes('interface')) {
            return responses.design[Math.floor(Math.random() * responses.design.length)];
        } else {
            return responses.general[Math.floor(Math.random() * responses.general.length)];
        }
    }

    setupAutoResponses() {
        // Add contextual suggestions based on current progress
        const suggestions = [
            "I noticed you're 65% through planning. Want me to review your business model?",
            "Your market validation is strong at 87%. Ready to discuss pricing strategy?",
            "I see you've been working for 2 hours today. Great dedication! Need any quick wins?",
            "Based on your competitor analysis, I have some differentiation ideas. Interested?"
        ];

        // Show random suggestion every 5 minutes
        setInterval(() => {
            if (!this.modal?.classList.contains('active')) {
                const suggestion = suggestions[Math.floor(Math.random() * suggestions.length)];
                this.showSuggestionNotification(suggestion);
            }
        }, 300000); // 5 minutes
    }

    showSuggestionNotification(message) {
        const notification = document.createElement('div');
        notification.className = 'coach-suggestion-notification';
        notification.innerHTML = `
            <div class="notification-content">
                <div class="coach-avatar-small">
                    <i class="fas fa-robot"></i>
                </div>
                <div class="suggestion-text">
                    <strong>Coach Alex</strong>
                    <p>${message}</p>
                </div>
                <button class="suggestion-close">&times;</button>
            </div>
        `;
        
        // Add notification styles
        const style = document.createElement('style');
        style.textContent = `
            .coach-suggestion-notification {
                position: fixed;
                bottom: 24px;
                right: 24px;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
                padding: 16px;
                border-radius: 12px;
                max-width: 300px;
                box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
                z-index: 9999;
                transform: translateY(100%);
                transition: transform 0.3s ease;
                cursor: pointer;
            }
            .coach-suggestion-notification.show {
                transform: translateY(0);
            }
            .notification-content {
                display: flex;
                gap: 12px;
                align-items: start;
            }
            .coach-avatar-small {
                width: 32px;
                height: 32px;
                background: rgba(255, 255, 255, 0.2);
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                font-size: 0.9rem;
                flex-shrink: 0;
            }
            .suggestion-text strong {
                font-weight: 600;
                font-size: 0.9rem;
            }
            .suggestion-text p {
                margin: 4px 0 0 0;
                font-size: 0.85rem;
                opacity: 0.9;
            }
            .suggestion-close {
                background: none;
                border: none;
                color: white;
                cursor: pointer;
                font-size: 1.2rem;
                opacity: 0.7;
                margin-left: auto;
            }
            .suggestion-close:hover {
                opacity: 1;
            }
        `;
        document.head.appendChild(style);
        
        document.body.appendChild(notification);
        
        // Animate in
        setTimeout(() => notification.classList.add('show'), 100);
        
        // Add click handlers
        notification.addEventListener('click', () => {
            this.openChat();
            notification.remove();
        });
        
        notification.querySelector('.suggestion-close').addEventListener('click', (e) => {
            e.stopPropagation();
            notification.style.transform = 'translateY(100%)';
            setTimeout(() => notification.remove(), 300);
        });
        
        // Auto-remove after 10 seconds
        setTimeout(() => {
            if (notification.parentNode) {
                notification.style.transform = 'translateY(100%)';
                setTimeout(() => notification.remove(), 300);
            }
        }, 10000);
    }
}

// Stats Counter Animation
class StatsAnimator {
    constructor() {
        this.init();
    }

    init() {
        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    this.animateNumber(entry.target);
                    observer.unobserve(entry.target);
                }
            });
        });

        const statNumbers = document.querySelectorAll('.stat-content h3');
        statNumbers.forEach(stat => observer.observe(stat));
    }

    animateNumber(element) {
        const text = element.textContent;
        const number = parseInt(text.replace(/[^0-9]/g, ''));
        const suffix = text.replace(/[0-9]/g, '');
        
        if (isNaN(number)) return;

        const duration = 2000;
        const increment = number / (duration / 16);
        let current = 0;

        const timer = setInterval(() => {
            current += increment;
            if (current >= number) {
                current = number;
                clearInterval(timer);
            }
            
            if (suffix.includes('K')) {
                element.textContent = Math.floor(current) + suffix;
            } else if (suffix.includes('%')) {
                element.textContent = Math.floor(current) + '%';
            } else {
                element.textContent = Math.floor(current) + suffix;
            }
        }, 16);
    }
}

// Navigation Enhancement
class NavigationEnhancer {
    constructor() {
        this.init();
    }

    init() {
        this.enhanceNavigation();
        this.addProgressIndicators();
    }

    enhanceNavigation() {
        const navItems = document.querySelectorAll('.nav-item:not(.disabled)');
        navItems.forEach(item => {
            item.addEventListener('click', () => {
                this.handleNavigation(item);
            });
        });
    }

    handleNavigation(clickedItem) {
        // Remove active class from all items
        document.querySelectorAll('.nav-item').forEach(item => {
            item.classList.remove('active');
        });
        
        // Add active class to clicked item
        clickedItem.classList.add('active');
        
        // Show navigation feedback
        const itemText = clickedItem.querySelector('span').textContent;
        this.showNavigationFeedback(itemText);
    }

    showNavigationFeedback(sectionName) {
        const feedback = document.createElement('div');
        feedback.className = 'nav-feedback';
        feedback.innerHTML = `
            <div class="feedback-content">
                <i class="fas fa-compass"></i>
                <span>Navigating to ${sectionName}...</span>
            </div>
        `;
        
        const style = document.createElement('style');
        style.textContent = `
            .nav-feedback {
                position: fixed;
                top: 50%;
                left: 50%;
                transform: translate(-50%, -50%);
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
                padding: 16px 24px;
                border-radius: 8px;
                z-index: 10000;
                opacity: 0;
                transition: opacity 0.3s ease;
            }
            .nav-feedback.show {
                opacity: 1;
            }
            .feedback-content {
                display: flex;
                align-items: center;
                gap: 12px;
                font-weight: 500;
            }
        `;
        document.head.appendChild(style);
        
        document.body.appendChild(feedback);
        setTimeout(() => feedback.classList.add('show'), 100);
        setTimeout(() => {
            feedback.style.opacity = '0';
            setTimeout(() => feedback.remove(), 300);
        }, 1500);
    }

    addProgressIndicators() {
        // Add subtle progress indicators to show completion status
        const progressItems = document.querySelectorAll('.nav-item .progress-indicator');
        progressItems.forEach(indicator => {
            const progressBar = indicator.querySelector('.progress-bar');
            const width = progressBar.style.width;
            progressBar.style.width = '0%';
            
            setTimeout(() => {
                progressBar.style.transition = 'width 1s ease-in-out';
                progressBar.style.width = width;
            }, 500);
        });
    }
}

// Global Functions
function closeCoachModal() {
    const modal = document.getElementById('coachModal');
    if (modal) {
        modal.classList.remove('active');
    }
}

function sendMessage() {
    if (window.coachChat) {
        window.coachChat.sendMessage();
    }
}

// Initialize everything when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Initialize all systems
    window.tooltipManager = new TooltipManager();
    window.progressAnimator = new ProgressAnimator();
    window.taskManager = new TaskManager();
    window.coachChat = new CoachChat();
    window.statsAnimator = new StatsAnimator();
    window.navigationEnhancer = new NavigationEnhancer();
    
    console.log('IdeaCoach Pro Dashboard initialized successfully!');
});