using MinimalAPI.Dominio.Entidades;

namespace Test.Domain.Entidades;

[TestClass]
public class VeiculoTest
{
    [TestMethod]
    public void TestarGetSetPropriedadesCarro()
    {
        //Arrage
        var veiculo = new Veiculo();

        //Act 
        veiculo.Id = 1;
        veiculo.Nome = "testeCarro";
        veiculo.Marca = "testec";
        veiculo.Ano = 2020;

        // Assert 
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual("testeCarro", veiculo.Nome);
        Assert.AreEqual("testec", veiculo.Marca);
        Assert.AreEqual(2020, veiculo.Ano);
    }
}
