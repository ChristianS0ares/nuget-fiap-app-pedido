using nuget_fiap_app_pedido_repository.Interface;


namespace nuget_fiap_app_pedido_repository.DB
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly RepositoryDB _session;

        public UnitOfWork(RepositoryDB session)
        {
            _session = session;
        }

    }
}
