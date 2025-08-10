namespace Jackson.Ideas.Mock.Models
{
    /// <summary>
    /// Funding option recommendation for business plan financing
    /// </summary>
    public class FundingOption
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // Equity, Debt, Grant, etc.
        public string FundingType { get; set; } = string.Empty; // Alternative alias for Type
        public string Description { get; set; } = string.Empty;
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public decimal RecommendedAmount { get; set; }
        public string TypicalAmountRange { get; set; } = string.Empty; // String representation of range
        public string Stage { get; set; } = string.Empty; // Seed, Series A, Growth, etc.
        public List<string> Requirements { get; set; } = new();
        public List<string> Advantages { get; set; } = new();
        public List<string> Disadvantages { get; set; } = new();
        public string Timeline { get; set; } = string.Empty;
        public int SuitabilityScore { get; set; }
        public FundingTerms Terms { get; set; } = new();
        public List<string> TypicalInvestors { get; set; } = new();
        public string ApplicationProcess { get; set; } = string.Empty;
    }

    /// <summary>
    /// Funding terms and conditions
    /// </summary>
    public class FundingTerms
    {
        public decimal InterestRate { get; set; }
        public string RepaymentTerms { get; set; } = string.Empty;
        public decimal EquityPercentage { get; set; }
        public string Valuation { get; set; } = string.Empty;
        public List<string> Conditions { get; set; } = new();
        public List<string> CovenantRequirements { get; set; } = new();
        public string GuaranteeRequirements { get; set; } = string.Empty;
        public string CollateralRequirements { get; set; } = string.Empty;
    }
}