namespace FISTNESSGYM.Services
{
    public class AuthorizationService
    {
        private readonly SecurityService _securityService;

        public AuthorizationService(SecurityService securityService)
        {
            _securityService = securityService;
        }

        public bool IsUserAuthenticated => _securityService.User != null && _securityService.IsAuthenticated();

        public bool IsUser => _securityService.IsInRole("Użytkownik");
        public bool IsClient => _securityService.IsInRole("Klient");
        public bool IsWorker => _securityService.IsInRole("Pracownik");
        public bool IsTrainer => _securityService.IsInRole("Trener");
        public bool IsAdmin => _securityService.IsInRole("Administrator");
    }

}
