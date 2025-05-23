﻿namespace ControleDeLicitacao.Domain.Ressources;

public class Modulo
{
    public int ID { get; }
    public string Nome { get; }

    public Modulo(int id, string nome)
    {
        ID = id;
        Nome = nome;
    }
}

public class Recurso
{
    public int ID { get; }
    public int ModuloID { get; }
    public string Nome { get; }
    public bool Permissao { get; }

    public Recurso(int id, int moduloId, string nome)
    {
        ID = id;
        ModuloID = moduloId;
        Nome = nome;
        Permissao = false;
    }
}
public static class PermissoesDeRecursos
{

    public static readonly List<Modulo> 
        Modulos = new List<Modulo>
        {
              new Modulo(id: 1, nome: "Entidades")
            , new Modulo(id: 2, nome: "Itens")
            , new Modulo(id: 3, nome: "Ata Licitação")
            , new Modulo(id: 4, nome: "Baixa")
            , new Modulo(id: 5, nome: "Notas")
            , new Modulo(id: 6, nome: "Usuarios")
            , new Modulo(id: 7, nome: "Auditoria")
            //, new Modulo(id: 8, nome: "Relatorios")
            //, new Modulo(id: 9, nome: "Cadastros Auxiliares")
        };

    public static readonly List<Recurso> 
        Recursos = new List<Recurso>
        {
            //-------------------------[Entidades]-------------------------
              new Recurso(id: 101, moduloId: 1, nome: "Visualizar")
            , new Recurso(id: 102, moduloId: 1, nome: "Nova Entidade")
            , new Recurso(id: 103, moduloId: 1, nome: "Editar Entidade")
            , new Recurso(id: 104, moduloId: 1, nome: "Inativar Entidade")
            //-------------------------------------------------------------

            //---------------------------[Itens]---------------------------
            , new Recurso(id: 201, moduloId: 2, nome: "Visualizar")
            , new Recurso(id: 202, moduloId: 2, nome: "Novo Item")
            , new Recurso(id: 203, moduloId: 2, nome: "Editar Item")
            , new Recurso(id: 204, moduloId: 2, nome: "Inativar Item")
            //-------------------------------------------------------------
            
            //-----------------------[Ata Licitação]-----------------------
            , new Recurso(id: 301, moduloId: 3, nome: "Visualizar")
            , new Recurso(id: 302, moduloId: 3, nome: "Nova Ata")
            , new Recurso(id: 303, moduloId: 3, nome: "Importar Ata")
            , new Recurso(id: 304, moduloId: 3, nome: "Editar Ata")
            , new Recurso(id: 305, moduloId: 3, nome: "Inativar Ata")
            , new Recurso(id: 306, moduloId: 3, nome: "Criar Reajuste")
            , new Recurso(id: 307, moduloId: 3, nome: "Excluir Reajuste")
            //-------------------------------------------------------------

            //---------------------------[Baixa]---------------------------
            , new Recurso(id: 401, moduloId: 4, nome: "Visualizar")
            , new Recurso(id: 407, moduloId: 4, nome: "Visualizar Empenho")
            , new Recurso(id: 402, moduloId: 4, nome: "Novo Empenho")
            , new Recurso(id: 406, moduloId: 4, nome: "Editar Empenho")
            , new Recurso(id: 403, moduloId: 4, nome: "Excluir Empenho")
            , new Recurso(id: 404, moduloId: 4, nome: "Importar Empenho")
            , new Recurso(id: 405, moduloId: 4, nome: "Inativar Empenho")
            //-------------------------------------------------------------

            //---------------------------[Notas]---------------------------
            , new Recurso(id: 501, moduloId: 5, nome: "Visualizar")
            , new Recurso(id: 502, moduloId: 5, nome: "Nova Nota")
            , new Recurso(id: 503, moduloId: 5, nome: "Editar Nota")
            , new Recurso(id: 504, moduloId: 5, nome: "Excluir Nota")
            //-------------------------------------------------------------

            //--------------------------[Usuarios]-------------------------
            , new Recurso(id: 601, moduloId: 6, nome: "Visualizar")
            , new Recurso(id: 602, moduloId: 6, nome: "Novo Usuário")
            , new Recurso(id: 603, moduloId: 6, nome: "Editar Usuário")
            , new Recurso(id: 604, moduloId: 6, nome: "Inativar Usuário")
            //-------------------------------------------------------------

            //-------------------------[Auditoria]-------------------------
            , new Recurso(id: 701, moduloId: 7, nome: "Visualizar")
            , new Recurso(id: 702, moduloId: 7, nome: "Nova Nota")
            , new Recurso(id: 703, moduloId: 7, nome: "Editar Nota")
            , new Recurso(id: 704, moduloId: 7, nome: "Inativar Nota")
            //-------------------------------------------------------------

        };
}