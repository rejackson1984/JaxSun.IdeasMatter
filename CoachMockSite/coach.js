// Coach-specific JavaScript functionality

// Demo scenarios data
const demoScenarios = {
    'idea-validation': [
        {
            type: 'user',
            content: "I have an idea for a productivity app, but I'm not sure if people actually want it."
        },
        {
            type: 'coach',
            content: "Great question! Let's validate your idea systematically. First, tell me about your productivity app - what specific problem does it solve that existing apps don't?",
            suggestions: ["Time management", "Task organization", "Team collaboration", "Focus/concentration"]
        },
        {
            type: 'user',
            content: "It helps people break down big projects into smaller, manageable tasks with AI suggestions."
        },
        {
            type: 'coach',
            content: "Interesting! The task breakdown market is worth $3.2B. I can see 3 ways to validate this:\n\n1. **Survey potential users** - I'll help you create a survey to test demand\n2. **Analyze competitors** - Let's see what gaps exist in current solutions\n3. **Build a simple prototype** - Test core functionality with real users\n\nWhich approach interests you most?",
            insights: [
                "Market size: $3.2B task management industry",
                "73% of people struggle with project breakdown",
                "AI-assisted planning is an emerging trend"
            ]
        }
    ],
    'market-research': [
        {
            type: 'user',
            content: "How do I research my target market effectively?"
        },
        {
            type: 'coach',
            content: "Excellent question! Market research is crucial for success. I'll guide you through my proven 4-step framework:\n\n**1. Define your market segments**\n**2. Analyze market size & trends**\n**3. Study your competition**\n**4. Validate with real customers**\n\nWhat's your business idea? I'll show you exactly how to research YOUR specific market.",
            suggestions: ["SaaS product", "E-commerce store", "Mobile app", "Service business"]
        }
    ],
    'business-plan': [
        {
            type: 'user',
            content: "I need help creating a business plan that investors will actually want to read."
        },
        {
            type: 'coach',
            content: "I'll help you create an investor-ready business plan! Based on analyzing 1,000+ successful funding rounds, here's what investors actually care about:\n\n**Must-have sections:**\n• Executive Summary (hook them in 2 minutes)\n• Market Opportunity (size + your slice)\n• Business Model (how you make money)\n• Financial Projections (realistic growth)\n• Team & Execution (why you'll win)\n\nI can generate each section with you. Which would you like to start with?",
            insights: [
                "Average investor spends 3.2 minutes on initial review",
                "Financial projections are #1 credibility factor",
                "Team section influences 40% of investment decisions"
            ]
        }
    ],
    'funding': [
        {
            type: 'user',
            content: "When should I raise funding and how much should I ask for?"
        },
        {
            type: 'coach',
            content: "Great timing question! Here's my framework for funding readiness:\n\n**Ready to raise when you have:**\n• Proven product-market fit\n• Clear path to profitability\n• 18-24 months runway needed\n• Strong team in place\n\n**How much to raise:**\n• Calculate 18-month burn rate\n• Add 25% buffer for delays\n• Include milestone achievement costs\n\nWhat stage is your business at? I'll give you a specific roadmap.",
            suggestions: ["Just an idea", "Have prototype", "Paying customers", "Growing revenue"]
        }
    ],
    'marketing': [
        {
            type: 'user',
            content: "I'm struggling to get customers. What marketing strategies actually work?"
        },
        {
            type: 'coach',
            content: "Customer acquisition is the #1 challenge for new businesses. Let me share what actually works:\n\n**High-ROI strategies for startups:**\n• Content marketing (3x cheaper than ads)\n• Referral programs (lowest CAC)\n• Partnership marketing (tap existing audiences)\n• SEO (long-term compound growth)\n\n**Avoid these common mistakes:**\n• Trying too many channels at once\n• Not tracking customer acquisition cost\n• Focusing on vanity metrics\n\nWhat's your target customer? I'll recommend the best channels for YOUR audience.",
            insights: [
                "Average startup tries 7 marketing channels",
                "Successful startups master 1-2 channels first",
                "Referrals have 4x higher conversion rates"
            ]
        }
    ]
};

// Initialize demo functionality
document.addEventListener('DOMContentLoaded', function() {
    initializeDemoScenarios();
    loadDefaultScenario();
});

function initializeDemoScenarios() {
    const scenarioButtons = document.querySelectorAll('.scenario-btn');
    scenarioButtons.forEach(button => {
        button.addEventListener('click', function() {
            // Remove active class from all buttons
            scenarioButtons.forEach(btn => btn.classList.remove('active'));
            // Add active class to clicked button
            this.classList.add('active');
            
            // Load the scenario
            const scenario = this.dataset.scenario;
            loadScenario(scenario);
        });
    });
}

function loadDefaultScenario() {
    loadScenario('idea-validation');
}

function loadScenario(scenarioKey) {
    const messages = demoScenarios[scenarioKey];
    if (!messages) return;
    
    const messagesContainer = document.getElementById('demoMessages');
    messagesContainer.innerHTML = '';
    
    messages.forEach((message, index) => {
        setTimeout(() => {
            addDemoMessage(message.content, message.type, message.suggestions, message.insights);
        }, index * 1000);
    });
}

function addDemoMessage(content, type, suggestions = null, insights = null) {
    const messagesContainer = document.getElementById('demoMessages');
    const messageDiv = document.createElement('div');
    messageDiv.className = `demo-message ${type}-message`;
    
    if (type === 'coach') {
        messageDiv.innerHTML = `
            <div class="message-avatar">
                <i class="fas fa-robot"></i>
            </div>
            <div class="message-content">
                <p>${content}</p>
                ${suggestions ? createSuggestions(suggestions) : ''}
                ${insights ? createInsights(insights) : ''}
            </div>
        `;
    } else {
        messageDiv.innerHTML = `
            <div class="message-content">
                <p>${content}</p>
            </div>
        `;
    }
    
    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

function createSuggestions(suggestions) {
    return `
        <div class="message-suggestions">
            ${suggestions.map(suggestion => 
                `<button class="suggestion-chip" onclick="selectSuggestion('${suggestion}')">${suggestion}</button>`
            ).join('')}
        </div>
    `;
}

function createInsights(insights) {
    return `
        <div class="message-insights">
            <h5><i class="fas fa-lightbulb"></i> Key Insights:</h5>
            <ul>
                ${insights.map(insight => `<li>${insight}</li>`).join('')}
            </ul>
        </div>
    `;
}

function selectSuggestion(suggestion) {
    const input = document.getElementById('demoInput');
    input.value = suggestion;
    sendDemoMessage();
}

function sendDemoMessage() {
    const input = document.getElementById('demoInput');
    const message = input.value.trim();
    if (!message) return;
    
    // Add user message
    addDemoMessage(message, 'user');
    input.value = '';
    
    // Generate coach response
    setTimeout(() => {
        const response = generateCoachResponse(message);
        addDemoMessage(response.content, 'coach', response.suggestions, response.insights);
    }, 1000 + Math.random() * 1500);
}

function generateCoachResponse(userMessage) {
    const responses = [
        {
            content: "That's a great question! Based on your situation, I'd recommend focusing on these key areas first. Let me break this down into actionable steps you can take right away.",
            suggestions: ["Tell me more", "What's next?", "Show me examples"],
            insights: ["This is a common challenge for 67% of entrepreneurs", "Success rate increases 3x with proper planning"]
        },
        {
            content: "I can see you're thinking strategically about this. Here's what successful entrepreneurs in similar situations have done. Let me share some specific tactics that work.",
            suggestions: ["Give me specifics", "What about my industry?", "Timeline for this?"],
            insights: ["Market data supports this approach", "Average implementation time: 2-3 weeks"]
        },
        {
            content: "Excellent insight! You're on the right track. Based on current market trends and your specific situation, I'd suggest these next steps to maximize your chances of success.",
            suggestions: ["How do I start?", "What resources do I need?", "Common mistakes to avoid?"],
            insights: ["This strategy has a 78% success rate", "ROI typically seen within 90 days"]
        }
    ];
    
    return responses[Math.floor(Math.random() * responses.length)];
}

// Modal functionality for demo chat
function openDemoChat() {
    const modal = document.getElementById('demoChatModal');
    if (modal) {
        modal.classList.add('active');
        document.getElementById('modalChatInput').focus();
    }
}

function closeDemoChat() {
    const modal = document.getElementById('demoChatModal');
    if (modal) {
        modal.classList.remove('active');
    }
}

function askDemo(question) {
    const input = document.getElementById('modalChatInput');
    input.value = question;
    sendModalMessage();
}

function sendModalMessage() {
    const input = document.getElementById('modalChatInput');
    const message = input.value.trim();
    if (!message) return;
    
    const messagesContainer = document.getElementById('modalChatMessages');
    
    // Add user message
    const userMessage = document.createElement('div');
    userMessage.className = 'message user-message';
    userMessage.innerHTML = `
        <div class="message-content">
            <p>${message}</p>
        </div>
    `;
    messagesContainer.appendChild(userMessage);
    
    input.value = '';
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
    
    // Generate AI response
    setTimeout(() => {
        const response = generateDetailedResponse(message);
        const coachMessage = document.createElement('div');
        coachMessage.className = 'message coach-message';
        coachMessage.innerHTML = `
            <div class="message-avatar">
                <i class="fas fa-robot"></i>
            </div>
            <div class="message-content">
                <p>${response}</p>
            </div>
        `;
        messagesContainer.appendChild(coachMessage);
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
    }, 1000 + Math.random() * 2000);
}

function generateDetailedResponse(userMessage) {
    const lowerMessage = userMessage.toLowerCase();
    
    if (lowerMessage.includes('validate') || lowerMessage.includes('idea')) {
        return "Validating your business idea is crucial! I'll help you through my 3-step validation framework: 1) Market research to understand demand, 2) Competitor analysis to find your unique position, 3) Customer interviews to validate assumptions. Want to start with market research?";
    } else if (lowerMessage.includes('market') || lowerMessage.includes('research')) {
        return "Market research doesn't have to be overwhelming! I'll guide you through analyzing your target market size, identifying customer segments, and understanding market trends. Based on your industry, I can recommend specific research tools and methodologies that work best.";
    } else if (lowerMessage.includes('business plan') || lowerMessage.includes('plan')) {
        return "I'll help you create a business plan that actually gets results! We'll focus on the sections investors care about most: market opportunity, business model, financial projections, and your go-to-market strategy. I can generate each section with you step by step.";
    } else if (lowerMessage.includes('funding') || lowerMessage.includes('money') || lowerMessage.includes('investment')) {
        return "Funding strategy depends on your business stage and goals. I'll help you determine if you're ready to raise capital, how much to ask for, and which type of investors to target. We'll also prepare your pitch materials and practice your presentation.";
    } else if (lowerMessage.includes('marketing') || lowerMessage.includes('customers') || lowerMessage.includes('sales')) {
        return "Customer acquisition is key to success! I'll help you identify the most effective marketing channels for your target audience, create a customer acquisition strategy, and set up systems to track and optimize your marketing ROI.";
    } else {
        return "That's a great question! I'm here to help you with all aspects of building a successful business. Whether you need help with market research, business planning, funding, marketing, or strategy, I can provide personalized guidance based on your specific situation. What's your biggest challenge right now?";
    }
}

// Add interactive animations
document.addEventListener('DOMContentLoaded', function() {
    // Animate coach card on scroll
    const observerOptions = {
        threshold: 0.2,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate-in');
            }
        });
    }, observerOptions);
    
    // Observe elements for animation
    const animatedElements = document.querySelectorAll('.coach-card, .capability, .feature-card, .scenario-card, .testimonial-card');
    animatedElements.forEach(el => observer.observe(el));
});

// Add styles for animations
const coachAnimationStyles = document.createElement('style');
coachAnimationStyles.textContent = `
    /* Demo Chat Styles */
    .demo-chat-modal {
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        z-index: 10000;
        display: none;
        align-items: center;
        justify-content: center;
    }
    
    .demo-chat-modal.active {
        display: flex;
    }
    
    .demo-chat-modal .modal-content {
        width: 90%;
        max-width: 700px;
        max-height: 80vh;
        background: white;
        border-radius: 16px;
        display: flex;
        flex-direction: column;
        box-shadow: 0 25px 50px rgba(0, 0, 0, 0.25);
    }
    
    /* Demo Messages */
    .demo-message {
        display: flex;
        gap: 12px;
        margin-bottom: 20px;
        align-items: start;
    }
    
    .demo-message.user-message {
        flex-direction: row-reverse;
        align-self: flex-end;
    }
    
    .demo-message .message-content {
        max-width: 70%;
        background: #f8fafc;
        padding: 16px;
        border-radius: 16px;
        border-bottom-left-radius: 4px;
    }
    
    .demo-message.user-message .message-content {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border-bottom-left-radius: 16px;
        border-bottom-right-radius: 4px;
    }
    
    .message-suggestions {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
        margin-top: 12px;
    }
    
    .suggestion-chip {
        background: rgba(102, 126, 234, 0.1);
        color: #667eea;
        border: 1px solid rgba(102, 126, 234, 0.2);
        padding: 6px 12px;
        border-radius: 20px;
        font-size: 0.85rem;
        cursor: pointer;
        transition: all 0.3s ease;
    }
    
    .suggestion-chip:hover {
        background: #667eea;
        color: white;
    }
    
    .message-insights {
        margin-top: 16px;
        padding: 16px;
        background: rgba(102, 126, 234, 0.03);
        border-left: 3px solid #667eea;
        border-radius: 8px;
    }
    
    .message-insights h5 {
        color: #667eea;
        margin-bottom: 8px;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 8px;
    }
    
    .message-insights ul {
        list-style: none;
        padding: 0;
        margin: 0;
    }
    
    .message-insights li {
        color: #1a1a1a;
        margin-bottom: 4px;
        padding-left: 16px;
        position: relative;
    }
    
    .message-insights li::before {
        content: '•';
        color: #667eea;
        font-weight: bold;
        position: absolute;
        left: 0;
    }
    
    /* Quick suggestions in modal */
    .quick-suggestions {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
        margin-top: 16px;
    }
    
    .suggestion-btn {
        background: #667eea;
        color: white;
        border: none;
        padding: 8px 16px;
        border-radius: 20px;
        font-size: 0.85rem;
        cursor: pointer;
        transition: all 0.3s ease;
    }
    
    .suggestion-btn:hover {
        background: #5a67d8;
        transform: translateY(-1px);
    }
    
    /* Animation classes */
    .coach-card, .capability, .feature-card, .scenario-card, .testimonial-card {
        opacity: 0;
        transform: translateY(30px);
        transition: all 0.6s ease;
    }
    
    .coach-card.animate-in, .capability.animate-in, .feature-card.animate-in, 
    .scenario-card.animate-in, .testimonial-card.animate-in {
        opacity: 1;
        transform: translateY(0);
    }
    
    /* Page-specific styles */
    .page-hero {
        padding: 120px 0 80px;
        background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
        text-align: center;
    }
    
    .coach-intro {
        padding: 100px 0;
        background: white;
    }
    
    .intro-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 80px;
        align-items: center;
    }
    
    .coach-card {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        border-radius: 20px;
        padding: 32px;
        box-shadow: 0 20px 40px rgba(102, 126, 234, 0.3);
    }
    
    .coach-header {
        display: flex;
        align-items: center;
        gap: 20px;
        margin-bottom: 32px;
    }
    
    .coach-avatar-large {
        width: 80px;
        height: 80px;
        background: rgba(255, 255, 255, 0.2);
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 2rem;
    }
    
    .coach-info h3 {
        margin: 0 0 4px 0;
        font-size: 1.5rem;
    }
    
    .coach-title {
        margin: 0 0 8px 0;
        opacity: 0.9;
    }
    
    .coach-status {
        display: flex;
        align-items: center;
        gap: 8px;
        font-size: 0.9rem;
    }
    
    .coach-stats {
        display: grid;
        gap: 20px;
        margin-bottom: 32px;
    }
    
    .stat-item {
        text-align: center;
    }
    
    .stat-number {
        display: block;
        font-size: 1.5rem;
        font-weight: 800;
        line-height: 1;
    }
    
    .stat-label {
        font-size: 0.85rem;
        opacity: 0.9;
        margin-top: 4px;
    }
    
    .coach-specialties h4 {
        margin-bottom: 16px;
        font-weight: 600;
    }
    
    .specialty-tags {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
    }
    
    .tag {
        background: rgba(255, 255, 255, 0.2);
        padding: 6px 12px;
        border-radius: 16px;
        font-size: 0.8rem;
        font-weight: 500;
    }
    
    /* Capabilities */
    .coach-capabilities {
        display: grid;
        gap: 24px;
    }
    
    .capability {
        display: flex;
        gap: 16px;
        align-items: start;
    }
    
    .capability-icon {
        width: 48px;
        height: 48px;
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        border-radius: 12px;
        display: flex;
        align-items: center;
        justify-content: center;
        color: white;
        font-size: 1.2rem;
        flex-shrink: 0;
    }
    
    .capability-content h4 {
        margin-bottom: 8px;
        color: #1a1a1a;
    }
    
    .capability-content p {
        color: #64748b;
        margin: 0;
    }
    
    /* Demo Section */
    .coach-demo-section {
        padding: 100px 0;
        background: #f8fafc;
    }
    
    .demo-interface {
        display: grid;
        grid-template-columns: 280px 1fr;
        gap: 32px;
        background: white;
        border-radius: 16px;
        overflow: hidden;
        box-shadow: 0 10px 25px rgba(0, 0, 0, 0.1);
    }
    
    .demo-sidebar {
        background: #f8fafc;
        padding: 32px;
    }
    
    .scenario-list {
        display: flex;
        flex-direction: column;
        gap: 8px;
    }
    
    .scenario-btn {
        display: flex;
        align-items: center;
        gap: 12px;
        padding: 16px;
        background: white;
        border: 2px solid transparent;
        border-radius: 8px;
        cursor: pointer;
        transition: all 0.3s ease;
        text-align: left;
        width: 100%;
    }
    
    .scenario-btn:hover {
        border-color: rgba(102, 126, 234, 0.2);
    }
    
    .scenario-btn.active {
        border-color: #667eea;
        background: rgba(102, 126, 234, 0.05);
        color: #667eea;
    }
    
    .demo-chat {
        display: flex;
        flex-direction: column;
        height: 600px;
    }
    
    .chat-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 24px;
        border-bottom: 1px solid rgba(0, 0, 0, 0.05);
    }
    
    .chat-status {
        display: flex;
        align-items: center;
        gap: 8px;
        color: #10b981;
        font-weight: 500;
    }
    
    #demoMessages {
        flex: 1;
        padding: 24px;
        overflow-y: auto;
        display: flex;
        flex-direction: column;
    }
    
    .chat-input-demo {
        padding: 24px;
        border-top: 1px solid rgba(0, 0, 0, 0.05);
        display: flex;
        gap: 12px;
    }
    
    .chat-input-demo input {
        flex: 1;
        padding: 12px 16px;
        border: 1px solid #e2e8f0;
        border-radius: 24px;
        outline: none;
        background: #f8fafc;
    }
    
    .chat-input-demo input:focus {
        border-color: #667eea;
        background: white;
    }
    
    .chat-input-demo button {
        width: 40px;
        height: 40px;
        background: #667eea;
        color: white;
        border: none;
        border-radius: 50%;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        transition: all 0.3s ease;
    }
    
    .chat-input-demo button:hover {
        background: #5a67d8;
        transform: scale(1.05);
    }
    
    /* Scenarios */
    .coaching-scenarios {
        padding: 100px 0;
        background: white;
    }
    
    .scenarios-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
        gap: 32px;
    }
    
    .scenario-card {
        background: #f8fafc;
        border-radius: 16px;
        padding: 24px;
        border: 1px solid rgba(0, 0, 0, 0.05);
    }
    
    .scenario-header {
        display: flex;
        align-items: center;
        gap: 16px;
        margin-bottom: 20px;
    }
    
    .user-avatar img {
        width: 50px;
        height: 50px;
        border-radius: 50%;
        object-fit: cover;
    }
    
    .scenario-info h4 {
        margin: 0 0 4px 0;
        color: #1a1a1a;
    }
    
    .scenario-info p {
        margin: 0;
        color: #64748b;
        font-size: 0.9rem;
    }
    
    .scenario-challenge {
        margin-bottom: 20px;
    }
    
    .scenario-challenge h5 {
        color: #ef4444;
        margin-bottom: 8px;
        font-weight: 600;
    }
    
    .scenario-challenge p {
        font-style: italic;
        color: #64748b;
        margin: 0;
    }
    
    .coach-response {
        display: flex;
        gap: 12px;
        margin-bottom: 20px;
        align-items: start;
    }
    
    .coach-avatar-mini {
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
    
    .response-content h5 {
        color: #667eea;
        margin-bottom: 8px;
        font-weight: 600;
    }
    
    .response-content p {
        margin-bottom: 12px;
        color: #1a1a1a;
    }
    
    .response-actions {
        display: flex;
        flex-wrap: wrap;
        gap: 6px;
    }
    
    .action-tag {
        background: rgba(102, 126, 234, 0.1);
        color: #667eea;
        padding: 4px 8px;
        border-radius: 12px;
        font-size: 0.75rem;
        font-weight: 500;
    }
    
    .scenario-outcome {
        background: rgba(16, 185, 129, 0.1);
        color: #059669;
        padding: 12px 16px;
        border-radius: 8px;
        border-left: 3px solid #10b981;
        font-weight: 500;
    }
    
    /* Testimonials */
    .coach-testimonials {
        padding: 100px 0;
        background: #f8fafc;
    }
    
    .testimonials-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(350px, 1fr));
        gap: 32px;
    }
    
    .testimonial-card {
        background: white;
        border-radius: 16px;
        padding: 32px;
        box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
    }
    
    .testimonial-content {
        margin-bottom: 24px;
    }
    
    .quote-icon {
        color: #667eea;
        font-size: 1.5rem;
        margin-bottom: 16px;
    }
    
    .testimonial-content p {
        color: #1a1a1a;
        font-style: italic;
        line-height: 1.6;
        margin: 0;
    }
    
    .testimonial-author {
        display: flex;
        align-items: center;
        gap: 16px;
    }
    
    .testimonial-author img {
        width: 60px;
        height: 60px;
        border-radius: 50%;
        object-fit: cover;
    }
    
    .author-info h5 {
        margin: 0 0 4px 0;
        color: #1a1a1a;
    }
    
    .author-info p {
        margin: 0;
        color: #64748b;
        font-size: 0.9rem;
    }
    
    /* Responsive Design */
    @media (max-width: 1024px) {
        .intro-grid {
            grid-template-columns: 1fr;
            gap: 48px;
        }
        
        .demo-interface {
            grid-template-columns: 1fr;
        }
        
        .demo-sidebar {
            order: -1;
        }
        
        .scenario-list {
            flex-direction: row;
            overflow-x: auto;
            padding-bottom: 16px;
        }
        
        .scenario-btn {
            min-width: 200px;
            flex-shrink: 0;
        }
    }
    
    @media (max-width: 768px) {
        .scenarios-grid, .testimonials-grid {
            grid-template-columns: 1fr;
        }
        
        .coach-card {
            padding: 24px;
        }
        
        .coach-header {
            flex-direction: column;
            text-align: center;
            gap: 16px;
        }
        
        .specialty-tags {
            justify-content: center;
        }
    }
`;
document.head.appendChild(coachAnimationStyles);