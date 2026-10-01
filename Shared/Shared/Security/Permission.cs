using Vero.Shared.DDD;

namespace Vero.Shared.Security
{
    public sealed class Permission : Enumeration<Permission>
    {
        /************************* COMMON **************************/
        public static readonly Permission COMMON_APP_ACCESS = new(1_000, "Dostęp do aplikacji");

        /************************* ASSIGNMENTS **************************/
        public static readonly Permission ASSIGNMENTS__ALLOW_MANAGE_ASSIGNMENTS = new(2_000, "Zarządzanie zadaniami");

        /************************* AUTH **************************/
        public static readonly Permission AUTH__ = new(3_000, "AUTH__TEMP_TO_REMOVE");

        /************************* CUSTOMERS **************************/
        public static readonly Permission CUSTOMERS__ = new(4_000, "CUSTOMERS__TEMP_TO_REMOVE");

        /************************* EVENTS **************************/
        public static readonly Permission EVENTS__ = new(5_000, "EVENTS__TEMP_TO_REMOVE");

        /************************* NOTIFICATIONS **************************/
        public static readonly Permission NOTIFICATIONS__ = new(6_000, "NOTIFICATIONS__TEMP_TO_REMOVE");

        /************************* OFFERS **************************/
        public static readonly Permission OFFERS__ = new(7_000, "OFFERS__TEMP_TO_REMOVE");

        /************************* TRANSPORTING **************************/
        public static readonly Permission TRANSPORTING__ = new(8_000, "TRANSPORTING__TEMP_TO_REMOVE");

        /************************* USERS **************************/
        public static readonly Permission USERS__ = new(9_000, "USERS__TEMP_TO_REMOVE");

        public static readonly Permission USERS__TEAM_MANAGER = new(9_100, "USERS__TEAM_MANAGER");
        public static readonly Permission USERS__TEAM_DEPUTY_MANAGER = new(9_101, "USERS__TEAM_DEPUTY_MANAGER");

        /************************* ACCOUNTING **************************/
        public static readonly Permission ACCOUNTING__ = new(10_000, "ACCOUNTING__TEMP_TO_REMOVE");

        /************************* MODULES_VISIBILITY **************************/
        public static readonly Permission MODULES_VISIBILITY__DESKTOP = new(11_000, "Pulpit");
        public static readonly Permission MODULES_VISIBILITY__OFFERS = new(11_100, "Oferty");
        public static readonly Permission MODULES_VISIBILITY__COMPLAINTS = new(11_200, "Reklamacje");
        public static readonly Permission MODULES_VISIBILITY__CUSTOMERS = new(11_300, "Klienci");
        public static readonly Permission MODULES_VISIBILITY__REGISTER_ACCOUNTING_DOCUMENT = new(11_400, "Rejestr dokumentów księgowych");
        public static readonly Permission MODULES_VISIBILITY__SUBCONTRACTORS = new(11_500, "Podwykonawcy");
        public static readonly Permission MODULES_VISIBILITY__ORDERS = new(11_600, "Zlecenia");
        public static readonly Permission MODULES_VISIBILITY__COST_DOCUMENTS = new(11_700, "Rejestr dokumentów kosztowych");
        public static readonly Permission MODULES_VISIBILITY__CASH_DOCUMENTS = new(11_800, "Kasa");
        public static readonly Permission MODULES_VISIBILITY__REPORTS = new(11_900, "Raporty");
        public static readonly Permission MODULES_VISIBILITY__BACKGROUND_SERVICES = new(12_000, "Tło rynkowe");
        public static readonly Permission MODULES_VISIBILITY__EXTERNAL_TABOR = new(12_100, "Tabor zewnętrzny");
        public static readonly Permission MODULES_VISIBILITY__EVENTS = new(12_200, "Kalendarz");
        public static readonly Permission MODULES_VISIBILITY__ADMIN_TASK_PANEL = new(12_300, "Panel zadań administratora");
        public static readonly Permission MODULES_VISIBILITY__COMPANY = new(12_350, "Firma");
        public static readonly Permission MODULES_VISIBILITY__DICTIONARIES = new(12_400, "Słowniki");
        public static readonly Permission MODULES_VISIBILITY__SETTINGS = new(12_500, "Ustawienia");

        private Permission(int id, string value) : base(id, value)
        {
        }
    }

    public sealed class PermissionGroup : Enumeration<PermissionGroup>
    {
        public static readonly PermissionGroup Common = new(1, "Ogólne", new[] { Permission.COMMON_APP_ACCESS });

        public static readonly PermissionGroup Assignments = new(2, "Zadania", new[] { Permission.ASSIGNMENTS__ALLOW_MANAGE_ASSIGNMENTS });

        public static readonly PermissionGroup Auth = new(3, "Autoryzacja", new[] { Permission.AUTH__ });

        public static readonly PermissionGroup Customers = new(4, "Klienci", new[] { Permission.CUSTOMERS__ });

        public static readonly PermissionGroup Events = new(5, "Zdarzenia", new[] { Permission.EVENTS__ });

        public static readonly PermissionGroup Notifications = new(6, "Powiadomienia", new[] { Permission.NOTIFICATIONS__ });

        public static readonly PermissionGroup Offers = new(7, "Oferty", new[] { Permission.OFFERS__ });

        public static readonly PermissionGroup Transporting = new(8, "Transport", new[] { Permission.TRANSPORTING__ });

        public static readonly PermissionGroup Users = new(
            9,
            "Użytkownicy",
            new[] { Permission.USERS__, Permission.USERS__TEAM_MANAGER, Permission.USERS__TEAM_DEPUTY_MANAGER }
        );

        public static readonly PermissionGroup Accounting = new(10, "Księgowość", new[] { Permission.ACCOUNTING__ });

        public static readonly PermissionGroup ModulesVisibility = new(
            100,
            "Widoczność modułów",
            new[]
            {
                Permission.MODULES_VISIBILITY__DESKTOP,
                Permission.MODULES_VISIBILITY__OFFERS,
                Permission.MODULES_VISIBILITY__COMPLAINTS,
                Permission.MODULES_VISIBILITY__CUSTOMERS,
                Permission.MODULES_VISIBILITY__REGISTER_ACCOUNTING_DOCUMENT,
                Permission.MODULES_VISIBILITY__SUBCONTRACTORS,
                Permission.MODULES_VISIBILITY__ORDERS,
                Permission.MODULES_VISIBILITY__COST_DOCUMENTS,
                Permission.MODULES_VISIBILITY__CASH_DOCUMENTS,
                Permission.MODULES_VISIBILITY__REPORTS,
                Permission.MODULES_VISIBILITY__BACKGROUND_SERVICES,
                Permission.MODULES_VISIBILITY__EXTERNAL_TABOR,
                Permission.MODULES_VISIBILITY__EVENTS,
                Permission.MODULES_VISIBILITY__ADMIN_TASK_PANEL,
                Permission.MODULES_VISIBILITY__COMPANY,
                Permission.MODULES_VISIBILITY__DICTIONARIES,
                Permission.MODULES_VISIBILITY__SETTINGS
            }
        );

        private PermissionGroup(int id, string value, IReadOnlyList<Permission> permissions) : base(id, value)
        {
            Permissions = permissions;
        }

        public IReadOnlyList<Permission> Permissions { get; }
    }
}