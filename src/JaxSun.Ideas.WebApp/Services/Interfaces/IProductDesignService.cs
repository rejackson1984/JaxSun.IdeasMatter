using JaxSun.Ideas.Mock.Models;

namespace JaxSun.Ideas.Mock.Services.Interfaces;

public interface IProductDesignService
{
    // Session Management
    Task<ProductDesignMockSession> CreateSessionAsync(string userId, string scenarioId, string sessionName);
    Task<ProductDesignMockSession?> GetSessionAsync(string sessionId);
    Task<List<ProductDesignMockSession>> GetUserSessionsAsync(string userId);
    Task<ProductDesignMockSession> UpdateSessionAsync(ProductDesignMockSession session);
    Task<bool> DeleteSessionAsync(string sessionId);
    
    // Question Flow
    Task<List<ProductDesignQuestion>> GetQuestionBankAsync();
    Task<ProductDesignQuestion?> GetNextQuestionAsync(string sessionId);
    Task<ProductDesignMockSession> SubmitAnswerAsync(string sessionId, string questionId, string answer);
    Task<bool> IsQuestionFlowCompleteAsync(string sessionId);
    
    // PRD Generation
    Task<string> GeneratePRDAsync(string sessionId);
    Task<ProductDesignMockSession> UpdatePRDAsync(string sessionId, string updatedPRD);
    Task<byte[]> ExportPRDAsPDFAsync(string sessionId);
    
    // Mockup Management
    Task<List<MockupTemplate>> GetMockupTemplatesAsync();
    Task<List<MockupTemplate>> GetRecommendedMockupsAsync(string sessionId);
    Task<MockupTemplate?> GetMockupTemplateAsync(string templateId);
    Task<ProductDesignMockSession> SelectMockupAsync(string sessionId, string templateId);
    
    // Design Modifications
    Task<string> ProcessModificationRequestAsync(string sessionId, string requestText);
    Task<List<DesignModificationRequest>> GetModificationHistoryAsync(string sessionId);
    Task<bool> UndoLastModificationAsync(string sessionId);
    
    // Requirements Management
    Task<List<ProductRequirement>> GetSessionRequirementsAsync(string sessionId);
    Task<ProductRequirement> AddRequirementAsync(string sessionId, ProductRequirement requirement);
    Task<ProductRequirement> UpdateRequirementAsync(ProductRequirement requirement);
    Task<bool> DeleteRequirementAsync(string requirementId);
}