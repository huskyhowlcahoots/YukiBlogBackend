namespace Blog.Api.Tests;

[TestFixture]
public class CalcTests
{
  private Calc _calc;

  [SetUp]
  public void Setup()
  {
    _calc = new Calc();
  }

  [Test]
  public void Add_WhenCalledWithPositiveNumbers_ReturnsCorrectSum()
  {
    // Arrange  
    int a = 5;
    int b = 3;

    // Act  
    int result = _calc.Add(a, b);

    // Assert  
    Assert.AreEqual(8, result);
  }

  [Test]
  public void Add_WhenCalledWithNegativeNumbers_ReturnsCorrectSum()
  {
    // Arrange  
    int a = -5;
    int b = -3;

    // Act  
    int result = _calc.Add(a, b);

    // Assert  
    Assert.AreEqual(-8, result);
  }

  [Test]
  public void Add_WhenCalledWithZero_ReturnsCorrectSum()
  {
    // Arrange  
    int a = 0;
    int b = 5;

    // Act  
    int result = _calc.Add(a, b);

    // Assert  
    Assert.AreEqual(5, result);
  }
}
