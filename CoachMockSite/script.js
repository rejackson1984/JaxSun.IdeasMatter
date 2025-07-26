// Navigation functionality
document.addEventListener('DOMContentLoaded', function() {
    const hamburger = document.getElementById('hamburger');
    const navMenu = document.getElementById('nav-menu');
    
    hamburger.addEventListener('click', function() {
        hamburger.classList.toggle('active');
        navMenu.classList.toggle('active');
    });
    
    // Close mobile menu when clicking on a link
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        link.addEventListener('click', () => {
            hamburger.classList.remove('active');
            navMenu.classList.remove('active');
        });
    });
});

// Smooth scrolling for navigation links
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({
                behavior: 'smooth',
                block: 'start'
            });
        }
    });
});

// Pricing toggle functionality
document.addEventListener('DOMContentLoaded', function() {
    const toggle = document.getElementById('pricing-toggle');
    const monthlyAmounts = document.querySelectorAll('.amount.monthly');
    const annualAmounts = document.querySelectorAll('.amount.annual');
    
    if (toggle) {
        toggle.addEventListener('change', function() {
            if (this.checked) {
                // Show annual pricing
                monthlyAmounts.forEach(amount => amount.style.display = 'none');
                annualAmounts.forEach(amount => amount.style.display = 'inline');
            } else {
                // Show monthly pricing
                monthlyAmounts.forEach(amount => amount.style.display = 'inline');
                annualAmounts.forEach(amount => amount.style.display = 'none');
            }
        });
    }
});

// Intersection Observer for animations
const observerOptions = {
    threshold: 0.1,
    rootMargin: '0px 0px -50px 0px'
};

const observer = new IntersectionObserver(function(entries) {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            entry.target.classList.add('visible');
        }
    });
}, observerOptions);

// Observe elements for animation
document.addEventListener('DOMContentLoaded', function() {
    const animatedElements = document.querySelectorAll('.fade-in, .slide-in-left, .slide-in-right');
    animatedElements.forEach(el => observer.observe(el));
});

// Chat interface simulation
document.addEventListener('DOMContentLoaded', function() {
    const chatMessages = document.querySelector('.chat-messages');
    const chatInput = document.querySelector('.chat-input input');
    const sendButton = document.querySelector('.chat-input button');
    
    if (chatInput && sendButton) {
        const sendMessage = () => {
            const message = chatInput.value.trim();
            if (message) {
                // Add user message
                const userMessage = document.createElement('div');
                userMessage.className = 'message user-message';
                userMessage.innerHTML = `
                    <div class="message-content">
                        <p>${message}</p>
                    </div>
                `;
                chatMessages.appendChild(userMessage);
                
                // Clear input
                chatInput.value = '';
                
                // Scroll to bottom
                chatMessages.scrollTop = chatMessages.scrollHeight;
                
                // Simulate AI response after delay
                setTimeout(() => {
                    const aiResponses = [
                        "That's a great question! Let me analyze that for you...",
                        "I can help you with that. Based on your business idea, here's what I recommend...",
                        "Interesting! I'm seeing some great opportunities in that market...",
                        "Let me pull up some data on that. Here's what the numbers show...",
                        "Perfect! That aligns well with current market trends..."
                    ];
                    
                    const randomResponse = aiResponses[Math.floor(Math.random() * aiResponses.length)];
                    
                    const aiMessage = document.createElement('div');
                    aiMessage.className = 'message coach-message';
                    aiMessage.innerHTML = `
                        <div class="message-avatar">
                            <i class="fas fa-robot"></i>
                        </div>
                        <div class="message-content">
                            <p>${randomResponse}</p>
                        </div>
                    `;
                    chatMessages.appendChild(aiMessage);
                    
                    // Scroll to bottom
                    chatMessages.scrollTop = chatMessages.scrollHeight;
                }, 1000 + Math.random() * 2000);
            }
        };
        
        sendButton.addEventListener('click', sendMessage);
        chatInput.addEventListener('keypress', function(e) {
            if (e.key === 'Enter') {
                sendMessage();
            }
        });
    }
});

// Action button handlers
document.addEventListener('DOMContentLoaded', function() {
    const actionButtons = document.querySelectorAll('.action-btn');
    actionButtons.forEach(button => {
        button.addEventListener('click', function() {
            const response = this.textContent;
            const chatMessages = document.querySelector('.chat-messages');
            
            // Add user response
            const userMessage = document.createElement('div');
            userMessage.className = 'message user-message';
            userMessage.innerHTML = `
                <div class="message-content">
                    <p>${response}</p>
                </div>
            `;
            chatMessages.appendChild(userMessage);
            
            // Remove the action buttons
            this.parentElement.remove();
            
            // Scroll to bottom
            chatMessages.scrollTop = chatMessages.scrollHeight;
            
            // Simulate AI follow-up
            setTimeout(() => {
                let followUp = "";
                if (response.includes("let's do it")) {
                    followUp = "Excellent! I'm starting the competitor analysis now. This will take about 30 seconds...";
                } else if (response.includes("data first")) {
                    followUp = "Smart approach! Here's the detailed breakdown of the fitness app market...";
                }
                
                if (followUp) {
                    const aiMessage = document.createElement('div');
                    aiMessage.className = 'message coach-message';
                    aiMessage.innerHTML = `
                        <div class="message-avatar">
                            <i class="fas fa-robot"></i>
                        </div>
                        <div class="message-content">
                            <p>${followUp}</p>
                        </div>
                    `;
                    chatMessages.appendChild(aiMessage);
                    chatMessages.scrollTop = chatMessages.scrollHeight;
                }
            }, 1500);
        });
    });
});

// Progress tracker simulation
document.addEventListener('DOMContentLoaded', function() {
    const phases = document.querySelectorAll('.phase');
    let currentPhase = 0;
    
    // Simulate progress over time
    setInterval(() => {
        if (currentPhase < phases.length - 1) {
            // Remove active class from current phase
            phases[currentPhase].classList.remove('active');
            phases[currentPhase].classList.remove('next');
            
            // Move to next phase
            currentPhase++;
            
            // Add active class to new current phase
            phases[currentPhase].classList.add('active');
            
            // Add next class to following phase if it exists
            if (currentPhase < phases.length - 1) {
                phases[currentPhase + 1].classList.add('next');
            }
        } else {
            // Reset to beginning
            phases.forEach(phase => {
                phase.classList.remove('active', 'next');
            });
            currentPhase = 0;
            phases[0].classList.add('active');
            if (phases.length > 1) {
                phases[1].classList.add('next');
            }
        }
    }, 3000); // Change phase every 3 seconds
});

// Navbar scroll effect
window.addEventListener('scroll', function() {
    const navbar = document.querySelector('.navbar');
    if (window.scrollY > 100) {
        navbar.style.background = 'rgba(255, 255, 255, 0.98)';
        navbar.style.boxShadow = '0 2px 20px rgba(0, 0, 0, 0.1)';
    } else {
        navbar.style.background = 'rgba(255, 255, 255, 0.95)';
        navbar.style.boxShadow = 'none';
    }
});

// Form validation for demo purposes
document.addEventListener('DOMContentLoaded', function() {
    const buttons = document.querySelectorAll('button');
    buttons.forEach(button => {
        if (button.textContent.includes('Start') || button.textContent.includes('Book')) {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                
                // Create modal or alert
                const modal = document.createElement('div');
                modal.style.cssText = `
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
                `;
                
                modal.innerHTML = `
                    <div style="
                        background: white;
                        padding: 40px;
                        border-radius: 16px;
                        text-align: center;
                        max-width: 400px;
                        margin: 20px;
                    ">
                        <div style="
                            width: 80px;
                            height: 80px;
                            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                            border-radius: 50%;
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            margin: 0 auto 20px;
                            font-size: 2rem;
                            color: white;
                        ">
                            <i class="fas fa-rocket"></i>
                        </div>
                        <h3 style="margin-bottom: 16px; color: #1a1a1a;">Demo Mode</h3>
                        <p style="color: #64748b; margin-bottom: 24px;">
                            This is a demo of the IdeaCoach Pro platform. In the real application, 
                            this would start your entrepreneurial journey!
                        </p>
                        <button onclick="this.parentElement.parentElement.remove()" style="
                            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                            color: white;
                            border: none;
                            padding: 12px 24px;
                            border-radius: 8px;
                            font-weight: 600;
                            cursor: pointer;
                        ">
                            Got it!
                        </button>
                    </div>
                `;
                
                document.body.appendChild(modal);
                
                // Close modal when clicking outside
                modal.addEventListener('click', function(e) {
                    if (e.target === modal) {
                        modal.remove();
                    }
                });
            });
        }
    });
});

// Stats counter animation
function animateCounter(element, target, duration = 2000) {
    const start = 0;
    const increment = target / (duration / 16);
    let current = start;
    
    const timer = setInterval(() => {
        current += increment;
        if (current >= target) {
            current = target;
            clearInterval(timer);
        }
        
        // Format number
        if (element.textContent.includes('$')) {
            if (target >= 1000000000) {
                element.textContent = '$' + (current / 1000000000).toFixed(1) + 'B+';
            } else if (target >= 1000000) {
                element.textContent = '$' + (current / 1000000).toFixed(1) + 'M+';
            } else if (target >= 1000) {
                element.textContent = '$' + (current / 1000).toFixed(0) + 'K';
            } else {
                element.textContent = '$' + Math.floor(current);
            }
        } else if (element.textContent.includes('%')) {
            element.textContent = Math.floor(current) + '%';
        } else if (element.textContent.includes('K')) {
            element.textContent = Math.floor(current / 1000) + 'K+';
        } else {
            element.textContent = Math.floor(current).toLocaleString() + '+';
        }
    }, 16);
}

// Animate stats when they come into view
const statsObserver = new IntersectionObserver((entries) => {
    entries.forEach(entry => {
        if (entry.isIntersecting) {
            const statNumber = entry.target;
            const text = statNumber.textContent;
            
            // Extract number from text
            let target = 0;
            if (text.includes('$2.3B')) target = 2300000000;
            else if (text.includes('15,000')) target = 15000;
            else if (text.includes('94%')) target = 94;
            else if (text.includes('73%')) target = 73;
            else if (text.includes('$847K')) target = 847000;
            else if (text.includes('312%')) target = 312;
            
            if (target > 0) {
                statNumber.textContent = '0';
                animateCounter(statNumber, target);
                statsObserver.unobserve(statNumber);
            }
        }
    });
});

document.addEventListener('DOMContentLoaded', function() {
    const statNumbers = document.querySelectorAll('.stat-number, .metric-number');
    statNumbers.forEach(stat => statsObserver.observe(stat));
});

// Login Modal Functions
function showLoginModal() {
    const modal = document.getElementById('loginModal');
    if (modal) {
        modal.classList.add('active');
        document.getElementById('email').focus();
    }
}

function closeLoginModal() {
    const modal = document.getElementById('loginModal');
    if (modal) {
        modal.classList.remove('active');
    }
}

function loginUser() {
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    
    // Demo login validation
    if (email === 'sarah@demo.com' && password === 'demo123') {
        // Show success message
        const successModal = document.createElement('div');
        successModal.className = 'success-modal';
        successModal.innerHTML = `
            <div class="modal-backdrop"></div>
            <div class="modal-content">
                <div class="success-content">
                    <div class="success-icon">
                        <i class="fas fa-check-circle"></i>
                    </div>
                    <h3>Welcome back, Sarah!</h3>
                    <p>Redirecting you to your dashboard...</p>
                    <div class="loading-bar">
                        <div class="loading-progress"></div>
                    </div>
                </div>
            </div>
        `;
        
        // Add success modal styles
        const style = document.createElement('style');
        style.textContent = `
            .success-modal {
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
            .success-modal .modal-content {
                background: white;
                border-radius: 16px;
                padding: 40px;
                text-align: center;
                max-width: 400px;
                margin: 20px;
                box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
            }
            .success-icon {
                width: 80px;
                height: 80px;
                background: linear-gradient(135deg, #10b981 0%, #059669 100%);
                border-radius: 50%;
                display: flex;
                align-items: center;
                justify-content: center;
                margin: 0 auto 20px;
                font-size: 2.5rem;
                color: white;
            }
            .success-content h3 {
                color: #1a1a1a;
                margin-bottom: 12px;
            }
            .success-content p {
                color: #64748b;
                margin-bottom: 24px;
            }
            .loading-bar {
                width: 100%;
                height: 4px;
                background: #e2e8f0;
                border-radius: 2px;
                overflow: hidden;
            }
            .loading-progress {
                width: 0%;
                height: 100%;
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                animation: loadingProgress 2s ease-in-out forwards;
            }
            @keyframes loadingProgress {
                to { width: 100%; }
            }
        `;
        document.head.appendChild(style);
        
        document.body.appendChild(successModal);
        
        // Redirect to dashboard after 2 seconds
        setTimeout(() => {
            window.location.href = 'dashboard.html';
        }, 2000);
        
    } else {
        // Show error message
        const errorDiv = document.createElement('div');
        errorDiv.className = 'login-error';
        errorDiv.innerHTML = `
            <i class="fas fa-exclamation-triangle"></i>
            <span>Invalid credentials. Use demo credentials: sarah@demo.com / demo123</span>
        `;
        
        const style = document.createElement('style');
        style.textContent = `
            .login-error {
                background: rgba(239, 68, 68, 0.1);
                border: 1px solid rgba(239, 68, 68, 0.2);
                color: #dc2626;
                padding: 12px 16px;
                border-radius: 8px;
                display: flex;
                align-items: center;
                gap: 8px;
                font-size: 0.9rem;
                margin-top: 16px;
            }
        `;
        document.head.appendChild(style);
        
        const loginForm = document.querySelector('.login-form');
        const existingError = loginForm.querySelector('.login-error');
        if (existingError) {
            existingError.remove();
        }
        loginForm.appendChild(errorDiv);
        
        // Remove error after 5 seconds
        setTimeout(() => {
            if (errorDiv.parentNode) {
                errorDiv.remove();
            }
        }, 5000);
    }
}

// Enhanced mobile menu styling
const style = document.createElement('style');
style.textContent = `
    @media (max-width: 768px) {
        .nav-menu {
            position: fixed;
            top: 80px;
            left: -100%;
            width: 100%;
            height: calc(100vh - 80px);
            background: rgba(255, 255, 255, 0.98);
            backdrop-filter: blur(20px);
            flex-direction: column;
            justify-content: start;
            align-items: center;
            padding: 40px 20px;
            transition: left 0.3s ease;
            border-top: 1px solid rgba(0, 0, 0, 0.05);
        }
        
        .nav-menu.active {
            left: 0;
        }
        
        .nav-link {
            font-size: 1.2rem;
            margin: 16px 0;
            padding: 12px 24px;
            border-radius: 8px;
            transition: all 0.3s ease;
        }
        
        .nav-link:hover {
            background: rgba(102, 126, 234, 0.1);
        }
        
        .nav-auth {
            flex-direction: column;
            gap: 16px;
            margin-top: 32px;
        }
        
        .nav-auth button {
            width: 200px;
            padding: 12px 24px;
        }
        
        .hamburger.active span:nth-child(1) {
            transform: rotate(45deg) translate(5px, 5px);
        }
        
        .hamburger.active span:nth-child(2) {
            opacity: 0;
        }
        
        .hamburger.active span:nth-child(3) {
            transform: rotate(-45deg) translate(7px, -6px);
        }
    }
`;
document.head.appendChild(style);