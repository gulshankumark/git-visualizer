// tests/GitVisualizer.Tests/Components/ScaffoldTests.cs
using Bunit;
using Xunit;

namespace GitVisualizer.Tests.Components;

public class ScaffoldTests : BunitContext
{
    [Fact]
    public void Scaffold_IsSetUp_bUnitTestContextAvailable()
    {
        // Placeholder: verifies bUnit BunitContext initialises correctly.
        // Full component tests begin in Story 2.1+.
        // Test method name pattern: [MethodUnderTest]_[Scenario]_[ExpectedResult]
        Assert.NotNull(Services);
    }
}
