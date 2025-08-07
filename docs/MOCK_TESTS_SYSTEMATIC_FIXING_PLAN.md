# Jackson.Ideas.Mock.Tests - Systematic Test Fixing Plan

## Overview
Comprehensive plan to systematically fix all 588 tests in Jackson.Ideas.Mock.Tests project using agent-driven TDD methodology with zero build error tolerance.

**Project**: `Jackson.Ideas.Mock.Tests`  
**Total Tests**: 588 tests across 32 test files  
**Goal**: 100% test pass rate with zero build errors  
**Methodology**: One test at a time, fix-build-verify cycle  

## Test Categories & Dependencies

### Layer 1: Foundation Models (No Dependencies)
**Priority**: Critical - Must pass first as other tests depend on these

#### 1.1 Core Models (19 tests)
- `Models/BusinessHubTests.cs`
- `Models/HubMetadataTests.cs` 
- `Models/HubContextTests.cs`

#### 1.2 Coaching Models (39 tests estimated)
- `Models/CoachingContextTests.cs`
- `Models/CoachingMessageTests.cs`
- `Models/CoachingSuggestionTests.cs`

### Layer 2: Core Services (Depend on Models)
**Priority**: High - Foundation services that other components depend on

#### 2.1 Configuration Services (76 tests)
- `Services/HubConfigurationServiceTests.cs` - Hub metadata and unlocking logic
- `Services/HubContextServiceTests.cs` - Hub context management

#### 2.2 Coaching Services (19 tests - **3 KNOWN FAILURES**)
- `Services/CoachPersonaServiceTests.cs` - **FAILING TESTS IDENTIFIED**
  - ❌ `GetCoachForHubAsync_ShouldReturn_SparkCoach_ForIdeaDevelopmentHub`
  - ❌ `GetCoachingSuggestionsAsync_ShouldInclude_SpecificSuggestionTypes`
  - ❌ `GetCoachMessageAsync_ShouldHandle_NullContext_Gracefully`

### Layer 3: Business Logic Services (Depend on Core Services)
**Priority**: High - Core business functionality

#### 3.1 Feature Services
- `Services/GamificationServiceTests.cs` (47 tests)
- `Services/QualityGateValidatorTests.cs` (estimated 25 tests)
- `Services/ProgressRewardTests.cs` (estimated 30 tests)

#### 3.2 Hub-Specific Services  
- `Features/Hub1/IdeaValidationServiceTests.cs`
- `Features/Hub2/BusinessPlanServiceTests.cs`
- `Features/Hub3/BusinessOperationsServiceTests.cs`

#### 3.3 Export & Generation Services
- `Services/DataExportServiceTests.cs` (12 tests)
- `Services/PdfGenerationServiceTests.cs`

### Layer 4: Component Tests (Depend on Services)
**Priority**: Medium - UI component functionality

#### 4.1 Hub Components
- `Components/Hub1/IdeaDevelopmentDashboardTests.cs`
- `Components/Hub1/IdeaInputComponentTests.cs`
- `Components/Hub2/BusinessPlanningDashboardTests.cs`
- `Components/Hub2/FinancialProjectionsComponentTests.cs`
- `Components/Hub3/BusinessOperationsDashboardTests.cs`
- `Components/Hub3/OperationsManagementComponentTests.cs`

#### 4.2 Navigation Components
- `Components/HubSelectorComponentTests.cs`
- `Components/HubNavMenuComponentTests.cs`
- `Components/DashboardComponentTests.cs`

### Layer 5: Integration Tests (Depend on All Layers)
**Priority**: Medium - End-to-end workflows

#### 5.1 Integration Tests
- `Integration/ServiceIntegrationTests.cs`
- `Integration/ThreeHubWorkflowIntegrationTests.cs`
- `Components/HubNavigationIntegrationTests.cs`

### Layer 6: Quality & Performance (Independent)
**Priority**: Low - Quality assurance tests

#### 6.1 Quality Tests
- `Quality/CodeQualityTests.cs`
- `Security/SecurityTests.cs`
- `Performance/PerformanceTests.cs`

## Agent Execution Plan

### Phase 1: Foundation Layer (Agent Commands)

**1.1 Core Models Analysis**
```
/agent software-engineer "Analyze and fix all tests in Jackson.Ideas.Mock.Tests Models layer:
- Models/BusinessHubTests.cs
- Models/HubMetadataTests.cs  
- Models/HubContextTests.cs
- Models/CoachingContextTests.cs
- Models/CoachingMessageTests.cs
- Models/CoachingSuggestionTests.cs

Apply systematic fix approach: analyze each failing test, implement fix, run dotnet build, verify test passes. Report all changes in detail."
```

**1.2 Foundation Services**
```
/agent software-engineer "Fix core configuration services in dependency order:
1. Services/HubConfigurationServiceTests.cs (76 tests)
2. Services/HubContextServiceTests.cs
Ensure each service test suite passes 100% before proceeding to next. Apply Golden Rule after each file."
```

### Phase 2: Coaching Services (Critical Failures)

**2.1 Fix Known Failing Tests**
```
/agent software-engineer "Fix the 3 known failing tests in Services/CoachPersonaServiceTests.cs:
1. GetCoachForHubAsync_ShouldReturn_SparkCoach_ForIdeaDevelopmentHub - Update Spark coach description to contain 'idea' keyword
2. GetCoachingSuggestionsAsync_ShouldInclude_SpecificSuggestionTypes - Add Warning type suggestions for BusinessOperations hub  
3. GetCoachMessageAsync_ShouldHandle_NullContext_Gracefully - Add null context handling in MockCoachPersonaService line 37

Fix ONE test at a time: analyze->fix->build->test->verify->next. Ensure all 19 tests pass before completion."
```

### Phase 3: Business Logic Services

**3.1 Feature Services**
```
/agent software-engineer "Fix business logic services in dependency order:
1. Services/GamificationServiceTests.cs (47 tests)
2. Services/QualityGateValidatorTests.cs  
3. Services/ProgressRewardTests.cs
Apply systematic approach for each service. Document all fixes."
```

**3.2 Hub-Specific Services**
```
/agent software-engineer "Fix hub-specific feature services:
1. Features/Hub1/IdeaValidationServiceTests.cs
2. Features/Hub2/BusinessPlanServiceTests.cs
3. Features/Hub3/BusinessOperationsServiceTests.cs
Ensure hub services integration works correctly."
```

**3.3 Export Services**
```
/agent software-engineer "Fix export and generation services:
1. Services/DataExportServiceTests.cs (12 tests - verify all pass)
2. Services/PdfGenerationServiceTests.cs
Focus on data integrity and format validation."
```

### Phase 4: Component Tests

**4.1 Hub Components**
```
/agent software-engineer "Fix hub component tests in order:
1. Hub1 components (IdeaDevelopmentDashboard, IdeaInput)
2. Hub2 components (BusinessPlanningDashboard, FinancialProjections)  
3. Hub3 components (BusinessOperationsDashboard, OperationsManagement)
Test component rendering and interaction logic."
```

**4.2 Navigation Components**
```
/agent software-engineer "Fix navigation component tests:
1. Components/HubSelectorComponentTests.cs
2. Components/HubNavMenuComponentTests.cs
3. Components/DashboardComponentTests.cs
Ensure proper routing and navigation behavior."
```

### Phase 5: Integration Tests

**5.1 Service Integration**
```
/agent software-engineer "Fix integration test suites:
1. Integration/ServiceIntegrationTests.cs
2. Integration/ThreeHubWorkflowIntegrationTests.cs  
3. Components/HubNavigationIntegrationTests.cs
Validate end-to-end workflows and service interactions."
```

### Phase 6: Quality & Performance

**6.1 Quality Assurance**
```
/agent software-engineer "Fix quality and performance test suites:
1. Quality/CodeQualityTests.cs
2. Security/SecurityTests.cs
3. Performance/PerformanceTests.cs
Ensure code quality standards and performance benchmarks."
```

## Success Criteria for Each Phase

### Build Requirements
- ✅ `./scripts/golden-rule.sh` must succeed with 0 errors before proceeding to next test
- ✅ Golden Rule Script validates: build + launch + application response
- ✅ All warnings should be documented but don't block progress
- ✅ Golden Rule must pass after each test file completion

### Test Requirements
- ✅ 100% pass rate for each test file before moving to next
- ✅ No test skips or ignores allowed
- ✅ All edge cases must be properly handled

### Documentation Requirements
- ✅ Each fix must be documented with root cause analysis
- ✅ Changes to production code must be justified
- ✅ Test improvements must be explained

## Execution Status Tracking

### Layer 1: Foundation Models ✅ **COMPLETED**
- [x] Models/BusinessHubTests.cs - **COMPLETED** ✅ 6/6 tests passed
- [x] Models/HubMetadataTests.cs - **COMPLETED** ✅ 8/8 tests passed  
- [x] Models/HubContextTests.cs - **COMPLETED** ✅ 11/11 tests passed
- [x] Models/CoachingContextTests.cs - **COMPLETED** ✅ 37/37 tests passed (Fixed progress calculation rounding)
- [x] Models/CoachingMessageTests.cs - **COMPLETED** ✅ 35/35 tests passed
- [x] Models/CoachingSuggestionTests.cs - **COMPLETED** ✅ 35/35 tests passed (Fixed null context handling)

**Layer 1 Summary**: 132/132 tests passed (100% success rate)
**Golden Rule Validation**: ✅ PASSED (Build + Launch + Validate)

#### Fixes Applied:
1. **CoachingContextTests.cs**: Fixed progress calculation test data - corrected expected value from 16% to 17% for 1/6 milestone completion (16.67% rounds to 17%)
2. **CoachingSuggestionTests.cs**: Added null context handling to `MockCoachPersonaService.GetIdeaDevelopmentSuggestions()`, `GetBusinessPlanningSuggestions()`, and `GetBusinessOperationsSuggestions()` methods

### Layer 2: Core Services  
- [ ] Services/HubConfigurationServiceTests.cs - **PENDING**
- [ ] Services/HubContextServiceTests.cs - **PENDING**
- [ ] Services/CoachPersonaServiceTests.cs - **PENDING** ⚠️ **3 KNOWN FAILURES**

### Layer 3: Business Logic
- [ ] Services/GamificationServiceTests.cs - **PENDING**
- [ ] Services/QualityGateValidatorTests.cs - **PENDING**
- [ ] Services/ProgressRewardTests.cs - **PENDING**
- [ ] Features/Hub1/IdeaValidationServiceTests.cs - **PENDING**
- [ ] Features/Hub2/BusinessPlanServiceTests.cs - **PENDING**
- [ ] Features/Hub3/BusinessOperationsServiceTests.cs - **PENDING**
- [ ] Services/DataExportServiceTests.cs - **PENDING**
- [ ] Services/PdfGenerationServiceTests.cs - **PENDING**

### Layer 4: Components
- [ ] Components/Hub1/* - **PENDING**
- [ ] Components/Hub2/* - **PENDING**
- [ ] Components/Hub3/* - **PENDING**
- [ ] Components/HubSelectorComponentTests.cs - **PENDING**
- [ ] Components/HubNavMenuComponentTests.cs - **PENDING**
- [ ] Components/DashboardComponentTests.cs - **PENDING**

### Layer 5: Integration
- [ ] Integration/ServiceIntegrationTests.cs - **PENDING**
- [ ] Integration/ThreeHubWorkflowIntegrationTests.cs - **PENDING**
- [ ] Components/HubNavigationIntegrationTests.cs - **PENDING**

### Layer 6: Quality
- [ ] Quality/CodeQualityTests.cs - **PENDING**
- [ ] Security/SecurityTests.cs - **PENDING**
- [ ] Performance/PerformanceTests.cs - **PENDING**

## Final Validation

### Complete Test Suite Validation
```
/agent software-engineer "Run complete Jackson.Ideas.Mock.Tests validation:
1. Execute full test suite: dotnet test tests/Jackson.Ideas.Mock.Tests/
2. Verify 588/588 tests pass (100% success rate)
3. Run Golden Rule script for application validation
4. Generate final completion report"
```

### Success Metrics
- ✅ **588/588 tests passing** (100% success rate)
- ✅ **Zero build errors** across entire solution
- ✅ **Golden Rule script passes** (build + launch + validate)
- ✅ **All production code changes documented** and justified
- ✅ **No test skips or ignores** remaining

---

## Start Execution

**To begin systematic test fixing, execute:**

```
/agent software-engineer "Begin Layer 1 foundation model tests analysis and fixing for Jackson.Ideas.Mock.Tests project"
```

This plan ensures methodical, dependency-aware test fixing with comprehensive documentation and zero tolerance for build errors.