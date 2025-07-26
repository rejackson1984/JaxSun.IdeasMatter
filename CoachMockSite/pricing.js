// Pricing page functionality

// ROI Calculator
function updateROICalculator() {
    const timeValue = parseFloat(document.getElementById('timeValue').value) || 50;
    const hoursWeek = parseFloat(document.getElementById('hoursWeek').value) || 20;
    const planCost = parseFloat(document.getElementById('planSelect').value) || 97;
    
    // Calculate based on 40% time savings (average reported by users)
    const timeSavedWeek = hoursWeek * 0.4;
    const timeSavedMonth = timeSavedWeek * 4.33; // Average weeks per month
    const valueSavedMonth = timeSavedMonth * timeValue;
    const netSavings = valueSavedMonth - planCost;
    const roi = ((netSavings / planCost) * 100);
    
    // Update display
    document.getElementById('timeSaved').textContent = `${timeSavedWeek.toFixed(1)} hours`;
    document.getElementById('valueSaved').textContent = `$${valueSavedMonth.toLocaleString()}/month`;
    document.getElementById('planCost').textContent = `$${planCost}/month`;
    document.getElementById('netSavings').textContent = `$${netSavings.toLocaleString()}`;
    document.getElementById('roiValue').textContent = `${roi.toFixed(0)}%`;
}

// FAQ Accordion
function initializeFAQ() {
    const faqItems = document.querySelectorAll('.faq-item');
    
    faqItems.forEach(item => {
        const question = item.querySelector('.faq-question');
        const answer = item.querySelector('.faq-answer');
        const icon = question.querySelector('i');
        
        question.addEventListener('click', () => {
            const isOpen = item.classList.contains('open');
            
            // Close all other FAQ items
            faqItems.forEach(otherItem => {
                otherItem.classList.remove('open');
                otherItem.querySelector('.faq-answer').style.maxHeight = '0';
                otherItem.querySelector('.faq-question i').style.transform = 'rotate(0deg)';
            });
            
            // Toggle current item
            if (!isOpen) {
                item.classList.add('open');
                answer.style.maxHeight = answer.scrollHeight + 'px';
                icon.style.transform = 'rotate(180deg)';
            }
        });
    });
}

// Plan Selection
function selectPlan(planType) {
    const planData = {
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
            description: 'For scaling businesses and agencies'
        }
    };
    
    const plan = planData[planType];
    
    if (planType === 'starter') {
        // Show free signup modal
        showSignupModal(plan);
    } else if (planType === 'enterprise') {
        // Show contact sales modal
        showContactModal(plan);
    } else {
        // Show trial signup modal
        showTrialModal(plan);
    }
}

function showSignupModal(plan) {
    const modal = createModal('signup', `
        <div class="signup-content">
            <div class="plan-summary">
                <h3>Start with ${plan.name}</h3>
                <p>${plan.description}</p>
                <div class="plan-price">Free Forever</div>
            </div>
            <form class="signup-form">
                <div class="form-group">
                    <label for="signupEmail">Email Address</label>
                    <input type="email" id="signupEmail" placeholder="Enter your email" required>
                </div>
                <div class="form-group">
                    <label for="signupName">Full Name</label>
                    <input type="text" id="signupName" placeholder="Enter your full name" required>
                </div>
                <div class="form-group">
                    <label for="signupPassword">Password</label>
                    <input type="password" id="signupPassword" placeholder="Create a password" required>
                </div>
                <div class="form-checkbox">
                    <label class="checkbox-label">
                        <input type="checkbox" required>
                        <span>I agree to the <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a></span>
                    </label>
                </div>
                <button type="submit" class="btn-modal-primary">
                    <i class="fas fa-rocket"></i>
                    Create Free Account
                </button>
            </form>
            <div class="signup-benefits">
                <h4>What you get with Starter:</h4>
                <ul>
                    <li><i class="fas fa-check"></i> 1 Business idea validation</li>
                    <li><i class="fas fa-check"></i> Basic market research</li>
                    <li><i class="fas fa-check"></i> AI coach interactions</li>
                    <li><i class="fas fa-check"></i> Community access</li>
                </ul>
            </div>
        </div>
    `);
}

function showTrialModal(plan) {
    const modal = createModal('trial', `
        <div class="trial-content">
            <div class="plan-summary">
                <h3>Start ${plan.name} Trial</h3>
                <p>${plan.description}</p>
                <div class="plan-price">
                    <span class="trial-price">Free for 14 days</span>
                    <span class="regular-price">then $${plan.price}/month</span>
                </div>
            </div>
            <form class="trial-form">
                <div class="form-group">
                    <label for="trialEmail">Email Address</label>
                    <input type="email" id="trialEmail" placeholder="Enter your email" required>
                </div>
                <div class="form-group">
                    <label for="trialName">Full Name</label>
                    <input type="text" id="trialName" placeholder="Enter your full name" required>
                </div>
                <div class="form-group">
                    <label for="trialCard">Credit Card</label>
                    <input type="text" id="trialCard" placeholder="1234 5678 9012 3456" required>
                    <div class="card-note">
                        <i class="fas fa-shield-alt"></i>
                        <span>You won't be charged until after your 14-day trial</span>
                    </div>
                </div>
                <div class="form-checkbox">
                    <label class="checkbox-label">
                        <input type="checkbox" required>
                        <span>I agree to the <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a></span>
                    </label>
                </div>
                <button type="submit" class="btn-modal-primary">
                    <i class="fas fa-crown"></i>
                    Start 14-Day Free Trial
                </button>
            </form>
            <div class="trial-features">
                <h4>Included in your trial:</h4>
                <ul>
                    <li><i class="fas fa-check"></i> Unlimited business ideas</li>
                    <li><i class="fas fa-check"></i> Advanced market research</li>
                    <li><i class="fas fa-check"></i> Complete business plan generation</li>
                    <li><i class="fas fa-check"></i> Unlimited AI coach access</li>
                    <li><i class="fas fa-check"></i> Priority support</li>
                </ul>
            </div>
            <div class="trial-guarantee">
                <i class="fas fa-shield-alt"></i>
                <span>30-day money-back guarantee • Cancel anytime</span>
            </div>
        </div>
    `);
}

function showContactModal(plan) {
    const modal = createModal('contact', `
        <div class="contact-content">
            <div class="plan-summary">
                <h3>${plan.name} Plan</h3>
                <p>${plan.description}</p>
                <div class="plan-price">Starting at $${plan.price}/month</div>
            </div>
            <form class="contact-form">
                <div class="form-row">
                    <div class="form-group">
                        <label for="contactFirstName">First Name</label>
                        <input type="text" id="contactFirstName" placeholder="John" required>
                    </div>
                    <div class="form-group">
                        <label for="contactLastName">Last Name</label>
                        <input type="text" id="contactLastName" placeholder="Doe" required>
                    </div>
                </div>
                <div class="form-group">
                    <label for="contactEmail">Business Email</label>
                    <input type="email" id="contactEmail" placeholder="john@company.com" required>
                </div>
                <div class="form-group">
                    <label for="contactCompany">Company Name</label>
                    <input type="text" id="contactCompany" placeholder="Your Company" required>
                </div>
                <div class="form-group">
                    <label for="contactTeamSize">Team Size</label>
                    <select id="contactTeamSize" required>
                        <option value="">Select team size</option>
                        <option value="1-5">1-5 people</option>
                        <option value="6-20">6-20 people</option>
                        <option value="21-100">21-100 people</option>
                        <option value="100+">100+ people</option>
                    </select>
                </div>
                <div class="form-group">
                    <label for="contactMessage">Tell us about your needs</label>
                    <textarea id="contactMessage" rows="4" placeholder="What are you looking to achieve with IdeaCoach Pro?"></textarea>
                </div>
                <button type="submit" class="btn-modal-primary">
                    <i class="fas fa-paper-plane"></i>
                    Request Demo & Pricing
                </button>
            </form>
            <div class="contact-benefits">
                <h4>What happens next:</h4>
                <ul>
                    <li><i class="fas fa-phone"></i> Our sales team will contact you within 2 hours</li>
                    <li><i class="fas fa-presentation"></i> We'll schedule a personalized demo</li>
                    <li><i class="fas fa-file-contract"></i> You'll receive custom pricing and terms</li>
                    <li><i class="fas fa-handshake"></i> Get onboarding support for your team</li>
                </ul>
            </div>
        </div>
    `);
}

function createModal(type, content) {
    const modal = document.createElement('div');
    modal.className = `plan-modal ${type}-modal`;
    modal.innerHTML = `
        <div class="modal-backdrop"></div>
        <div class="modal-content">
            <div class="modal-header">
                <button class="modal-close" onclick="this.closest('.plan-modal').remove()">
                    <i class="fas fa-times"></i>
                </button>
            </div>
            <div class="modal-body">
                ${content}
            </div>
        </div>
    `;
    
    // Add modal styles
    const modalStyles = document.createElement('style');
    modalStyles.textContent = `
        .plan-modal {
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            z-index: 10000;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .plan-modal .modal-content {
            background: white;
            border-radius: 16px;
            width: 90%;
            max-width: 500px;
            max-height: 90vh;
            overflow-y: auto;
            box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
        }
        .plan-modal .modal-header {
            padding: 20px 20px 0;
            display: flex;
            justify-content: flex-end;
        }
        .plan-modal .modal-body {
            padding: 0 20px 20px;
        }
        .plan-summary {
            text-align: center;
            margin-bottom: 32px;
        }
        .plan-summary h3 {
            color: #1a1a1a;
            margin-bottom: 8px;
        }
        .plan-summary p {
            color: #64748b;
            margin-bottom: 16px;
        }
        .plan-price {
            font-size: 1.25rem;
            font-weight: 700;
            color: #667eea;
        }
        .trial-price {
            color: #10b981;
        }
        .regular-price {
            font-size: 0.9rem;
            color: #64748b;
            display: block;
            margin-top: 4px;
        }
        .signup-form, .trial-form, .contact-form {
            display: flex;
            flex-direction: column;
            gap: 20px;
            margin-bottom: 24px;
        }
        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 16px;
        }
        .form-group {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }
        .form-group label {
            font-weight: 600;
            color: #1a1a1a;
            font-size: 0.9rem;
        }
        .form-group input, .form-group select, .form-group textarea {
            padding: 12px 16px;
            border: 2px solid #e2e8f0;
            border-radius: 8px;
            font-size: 1rem;
            transition: all 0.3s ease;
            background: #f8fafc;
        }
        .form-group input:focus, .form-group select:focus, .form-group textarea:focus {
            outline: none;
            border-color: #667eea;
            background: white;
            box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
        }
        .card-note {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-top: 8px;
            font-size: 0.85rem;
            color: #10b981;
        }
        .form-checkbox {
            display: flex;
            align-items: start;
            gap: 12px;
        }
        .btn-modal-primary {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            border: none;
            padding: 14px 20px;
            border-radius: 8px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
            font-size: 1rem;
        }
        .btn-modal-primary:hover {
            transform: translateY(-2px);
            box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
        }
        .signup-benefits, .trial-features, .contact-benefits {
            background: #f8fafc;
            border-radius: 8px;
            padding: 20px;
        }
        .signup-benefits h4, .trial-features h4, .contact-benefits h4 {
            color: #1a1a1a;
            margin-bottom: 12px;
            font-weight: 600;
        }
        .signup-benefits ul, .trial-features ul, .contact-benefits ul {
            list-style: none;
            padding: 0;
            margin: 0;
        }
        .signup-benefits li, .trial-features li, .contact-benefits li {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 8px;
            color: #64748b;
            font-size: 0.9rem;
        }
        .signup-benefits i, .trial-features i, .contact-benefits i {
            color: #10b981;
            width: 16px;
        }
        .trial-guarantee {
            text-align: center;
            margin-top: 16px;
            padding-top: 16px;
            border-top: 1px solid #e2e8f0;
            color: #64748b;
            font-size: 0.9rem;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 8px;
        }
        .trial-guarantee i {
            color: #10b981;
        }
    `;
    document.head.appendChild(modalStyles);
    
    document.body.appendChild(modal);
    
    // Handle form submission
    const form = modal.querySelector('form');
    if (form) {
        form.addEventListener('submit', function(e) {
            e.preventDefault();
            handleFormSubmission(type, form);
        });
    }
    
    // Close modal when clicking backdrop
    modal.querySelector('.modal-backdrop').addEventListener('click', () => {
        modal.remove();
    });
    
    return modal;
}

function handleFormSubmission(type, form) {
    // Simulate form processing
    const submitButton = form.querySelector('button[type="submit"]');
    const originalText = submitButton.innerHTML;
    
    submitButton.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
    submitButton.disabled = true;
    
    setTimeout(() => {
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
                    <h3>${getSuccessTitle(type)}</h3>
                    <p>${getSuccessMessage(type)}</p>
                    <button onclick="this.closest('.success-modal').remove(); window.location.href='dashboard.html';" class="btn-success">
                        ${type === 'contact' ? 'Got it!' : 'Go to Dashboard'}
                    </button>
                </div>
            </div>
        `;
        
        // Add success modal styles
        const successStyles = document.createElement('style');
        successStyles.textContent = `
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
            .btn-success {
                background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                color: white;
                border: none;
                padding: 12px 24px;
                border-radius: 8px;
                font-weight: 600;
                cursor: pointer;
            }
        `;
        document.head.appendChild(successStyles);
        
        // Remove original modal and show success
        form.closest('.plan-modal').remove();
        document.body.appendChild(successModal);
        
    }, 2000);
}

function getSuccessTitle(type) {
    switch(type) {
        case 'signup': return 'Welcome to IdeaCoach Pro!';
        case 'trial': return 'Trial Started Successfully!';
        case 'contact': return 'Request Received!';
        default: return 'Success!';
    }
}

function getSuccessMessage(type) {
    switch(type) {
        case 'signup': return 'Your free account has been created. Start validating your business idea today!';
        case 'trial': return 'Your 14-day trial has started. You have full access to all Professional features.';
        case 'contact': return 'Our sales team will contact you within 2 hours to schedule your demo.';
        default: return 'Your request has been processed successfully.';
    }
}

// Initialize everything when page loads
document.addEventListener('DOMContentLoaded', function() {
    // Initialize ROI calculator
    updateROICalculator();
    
    // Bind calculator inputs
    ['timeValue', 'hoursWeek', 'planSelect'].forEach(id => {
        const element = document.getElementById(id);
        if (element) {
            element.addEventListener('input', updateROICalculator);
            element.addEventListener('change', updateROICalculator);
        }
    });
    
    // Initialize FAQ
    initializeFAQ();
    
    // Initialize pricing toggle (reuse from main script)
    const toggle = document.getElementById('pricing-toggle');
    if (toggle) {
        toggle.addEventListener('change', function() {
            const monthlyAmounts = document.querySelectorAll('.amount.monthly, .price-note.monthly');
            const annualAmounts = document.querySelectorAll('.amount.annual, .price-note.annual');
            
            if (this.checked) {
                monthlyAmounts.forEach(el => el.style.display = 'none');
                annualAmounts.forEach(el => el.style.display = 'inline');
            } else {
                monthlyAmounts.forEach(el => el.style.display = 'inline');
                annualAmounts.forEach(el => el.style.display = 'none');
            }
        });
    }
});