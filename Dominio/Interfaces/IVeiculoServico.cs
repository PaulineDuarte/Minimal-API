using MinimalAPI.Dominio.Entidades;
using MinimalAPI.DTOs;

namespace Minimal_API.Dominio.interfaces ;

public interface IVeiculoServico
{
    List<Veiculo> Todos(int pagina , string nome , string marca);
    Veiculo BuscaPorId(int Id);

    void Incluir(Veiculo veiculo) ; 

    void Atualizar(Veiculo veiculo);

    void Apagar(Veiculo veiculo); 
}