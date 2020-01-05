namespace InfiniteRoleplay
{
    public enum TipoMensagem
    {
        Nenhum,
        Erro,
        Sucesso,
        Titulo,
        Punicao,
    }

    public enum TipoMensagemJogo
    {
        Me,
        Do,
        ChatICNormal,
        ChatICGrito,
        ChatOOC,
        ChatICBaixo,
    }

    public enum TipoFaccao
    {
        Policial = 1,
        Medica = 2,
        Criminosa = 3,
    }

    public enum TipoPunicao
    {
        Kick = 1,
        Ban = 2,
    }

    public enum TipoLog
    {
        Staff = 1,
        FaccaoLider = 2,
        FaccaoGestor = 3,
    }

    public enum TipoConvite
    {
        Faccao = 1,
    }
}