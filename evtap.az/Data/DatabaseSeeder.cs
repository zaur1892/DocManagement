using EvtapAz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EvtapAz.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            await context.Database.MigrateAsync();

            // Roles
            foreach (var role in new[] { "Admin", "User" })
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // Admin user
            if (await userManager.FindByNameAsync("admin") == null)
            {
                var admin = new IdentityUser { UserName = "admin", Email = "admin@evtap.az", EmailConfirmed = true };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded) await userManager.AddToRoleAsync(admin, "Admin");
            }

            // ===== DEMO İSTİFADƏÇİLƏR VƏ ELANLAR =====
            await SeedDemoListingsAsync(context, userManager);

            if (context.Cities.Any()) return;

            // ===== ŞƏHƏRLƏR VƏ RAYONLAR =====
            var cities = new List<City>
            {
                new() { Name="Bakı",        NameRu="Баку",          NameEn="Baku",        HasDistricts=true, HasMetro=true, SortOrder=1 },
                new() { Name="Sumqayıt",    NameRu="Сумгайыт",      NameEn="Sumqayit",    SortOrder=2 },
                new() { Name="Gəncə",       NameRu="Гянджа",        NameEn="Ganja",       SortOrder=3 },
                new() { Name="Mingəçevir",  NameRu="Мингечевир",    NameEn="Mingachevir", SortOrder=4 },
                new() { Name="Naxçıvan",    NameRu="Нахчыван",      NameEn="Nakhchivan",  SortOrder=5 },
                new() { Name="Şirvan",      NameRu="Ширван",        NameEn="Shirvan",     SortOrder=6 },
                new() { Name="Lənkəran",    NameRu="Ленкорань",     NameEn="Lankaran",    SortOrder=7 },
                new() { Name="Şəki",        NameRu="Шеки",          NameEn="Sheki",       SortOrder=8 },
                new() { Name="Yevlax",      NameRu="Евлах",         NameEn="Yevlakh",     SortOrder=9 },
                new() { Name="Naftalan",    NameRu="Нафталан",      NameEn="Naftalan",    SortOrder=10 },
                new() { Name="Abşeron r.",  NameRu="Абшеронский р.",  SortOrder=11 },
                new() { Name="Ağcabədi r.", NameRu="Агджабединский р.", SortOrder=12 },
                new() { Name="Ağdam r.",    NameRu="Агдамский р.",   SortOrder=13 },
                new() { Name="Ağdaş r.",    NameRu="Агдашский р.",   SortOrder=14 },
                new() { Name="Ağstafa r.",  NameRu="Агстафинский р.", SortOrder=15 },
                new() { Name="Ağsu r.",     NameRu="Агсуинский р.",  SortOrder=16 },
                new() { Name="Astara r.",   NameRu="Астаринский р.", SortOrder=17 },
                new() { Name="Balakən r.",  NameRu="Балакенский р.", SortOrder=18 },
                new() { Name="Bərdə r.",    NameRu="Бардинский р.",  SortOrder=19 },
                new() { Name="Beyləqan r.", NameRu="Бейлаганский р.", SortOrder=20 },
                new() { Name="Biləsuvar r.",NameRu="Билясуварский р.", SortOrder=21 },
                new() { Name="Cəbrayıl r.", NameRu="Джебраильский р.", SortOrder=22 },
                new() { Name="Cəlilabad r.",NameRu="Джалилабадский р.", SortOrder=23 },
                new() { Name="Daşkəsən r.", NameRu="Дашкесанский р.", SortOrder=24 },
                new() { Name="Füzuli r.",   NameRu="Физулинский р.", SortOrder=25 },
                new() { Name="Gədəbəy r.",  NameRu="Гедабекский р.", SortOrder=26 },
                new() { Name="Goranboy r.", NameRu="Горанбойский р.", SortOrder=27 },
                new() { Name="Göyçay r.",   NameRu="Гейчайский р.",  SortOrder=28 },
                new() { Name="Göygöl r.",   NameRu="Гейгельский р.", SortOrder=29 },
                new() { Name="Hacıqabul r.",NameRu="Гаджигабульский р.", SortOrder=30 },
                new() { Name="İmişli r.",   NameRu="Имишлинский р.", SortOrder=31 },
                new() { Name="İsmayıllı r.",NameRu="Исмаиллинский р.", SortOrder=32 },
                new() { Name="Kəlbəcər r.", NameRu="Кельбаджарский р.", SortOrder=33 },
                new() { Name="Kürdəmir r.", NameRu="Кюрдамирский р.", SortOrder=34 },
                new() { Name="Laçın r.",    NameRu="Лачинский р.",   SortOrder=35 },
                new() { Name="Lerik r.",    NameRu="Лерикский р.",   SortOrder=36 },
                new() { Name="Masallı r.",  NameRu="Масаллинский р.", SortOrder=37 },
                new() { Name="Neftçala r.", NameRu="Нефтечалинский р.", SortOrder=38 },
                new() { Name="Oğuz r.",     NameRu="Огузский р.",    SortOrder=39 },
                new() { Name="Qax r.",      NameRu="Кахский р.",     SortOrder=40 },
                new() { Name="Qazax r.",    NameRu="Казахский р.",   SortOrder=41 },
                new() { Name="Qəbələ r.",   NameRu="Габалинский р.", SortOrder=42 },
                new() { Name="Qobustan r.", NameRu="Гобустанский р.", SortOrder=43 },
                new() { Name="Quba r.",     NameRu="Кубинский р.",   SortOrder=44 },
                new() { Name="Qubadlı r.",  NameRu="Губадлинский р.", SortOrder=45 },
                new() { Name="Qusar r.",    NameRu="Кусарский р.",   SortOrder=46 },
                new() { Name="Saatlı r.",   NameRu="Саатлинский р.", SortOrder=47 },
                new() { Name="Sabirabad r.",NameRu="Сабирабадский р.", SortOrder=48 },
                new() { Name="Salyan r.",   NameRu="Сальянский р.",  SortOrder=49 },
                new() { Name="Şamaxı r.",   NameRu="Шемахинский р.", SortOrder=50 },
                new() { Name="Şəmkir r.",   NameRu="Шамкирский р.",  SortOrder=51 },
                new() { Name="Samux r.",    NameRu="Самухский р.",   SortOrder=52 },
                new() { Name="Siyəzən r.",  NameRu="Сиязанский р.",  SortOrder=53 },
                new() { Name="Şuşa r.",     NameRu="Шушинский р.",   SortOrder=54 },
                new() { Name="Tərtər r.",   NameRu="Тертерский р.",  SortOrder=55 },
                new() { Name="Tovuz r.",    NameRu="Товузский р.",   SortOrder=56 },
                new() { Name="Ucar r.",     NameRu="Уджарский р.",   SortOrder=57 },
                new() { Name="Xaçmaz r.",   NameRu="Хачмазский р.",  SortOrder=58 },
                new() { Name="Xızı r.",     NameRu="Хызынский р.",   SortOrder=59 },
                new() { Name="Xocavənd r.", NameRu="Ходжавендский р.", SortOrder=60 },
                new() { Name="Yardımlı r.", NameRu="Ярдымлинский р.", SortOrder=61 },
                new() { Name="Zaqatala r.", NameRu="Закатальский р.", SortOrder=62 },
                new() { Name="Zərdab r.",   NameRu="Зардабский р.",  SortOrder=63 },
                new() { Name="Zəngilan r.", NameRu="Зангиланский р.", SortOrder=64 },
            };
            await context.Cities.AddRangeAsync(cities);
            await context.SaveChangesAsync();

            // Get Baku's auto-generated Id
            var bakuId = (await context.Cities.FirstAsync(c => c.Name == "Bakı")).Id;

            // ===== BAKI RAYONLARİ =====
            var districts = new List<District>
            {
                new() { CityId=bakuId, Name="Binagadi",  NameRu="Бинагади",  SortOrder=1 },
                new() { CityId=bakuId, Name="Qaradağ",   NameRu="Гарадаг",   SortOrder=2 },
                new() { CityId=bakuId, Name="Xətai",     NameRu="Хатаи",     SortOrder=3 },
                new() { CityId=bakuId, Name="Xəzər",     NameRu="Хазар",     SortOrder=4 },
                new() { CityId=bakuId, Name="Nərimanov", NameRu="Нариманов", SortOrder=5 },
                new() { CityId=bakuId, Name="Nəsimi",    NameRu="Насими",    SortOrder=6 },
                new() { CityId=bakuId, Name="Nizami",    NameRu="Низами",    SortOrder=7 },
                new() { CityId=bakuId, Name="Pirallahı", NameRu="Пираллахи", SortOrder=8 },
                new() { CityId=bakuId, Name="Sabail",    NameRu="Сабаил",    SortOrder=9 },
                new() { CityId=bakuId, Name="Sabunçu",   NameRu="Сабунчу",   SortOrder=10 },
                new() { CityId=bakuId, Name="Suraxanı",  NameRu="Сураханы",  SortOrder=11 },
            };
            await context.Districts.AddRangeAsync(districts);
            await context.SaveChangesAsync();

            // Get district Ids by name
            var districtMap = await context.Districts
                .Where(d => d.CityId == bakuId)
                .ToDictionaryAsync(d => d.Name, d => d.Id);

            // ===== QƏSƏBƏLƏr =====
            var settlements = new List<Settlement>();
            void AddS(string distName, string name, string nameRu) {
                if (districtMap.TryGetValue(distName, out var dId))
                    settlements.Add(new() { DistrictId=dId, Name=name, NameRu=nameRu });
            }
            AddS("Binagadi",  "Binəqədi qəs.",   "пос. Бинагади");
            AddS("Binagadi",  "Biləcəri qəs.",   "пос. Биладжары");
            AddS("Binagadi",  "Balaxanı qəs.",   "пос. Балаханы");
            AddS("Binagadi",  "Zabrat qəs.",     "пос. Забрат");
            AddS("Binagadi",  "Maştağa qəs.",    "пос. Маштаги");
            AddS("Xəzər",     "Buzovna qəs.",    "пос. Бузовна");
            AddS("Xəzər",     "Novxanı qəs.",    "пос. Новханы");
            AddS("Xəzər",     "Pirəkəşkül qəs.","пос. Пиракешкюль");
            AddS("Xəzər",     "Nardaran qəs.",   "пос. Нардаран");
            AddS("Xəzər",     "Corat qəs.",      "пос. Джорат");
            AddS("Sabunçu",   "Sabunçu qəs.",    "пос. Сабунчу");
            AddS("Sabunçu",   "Ramana qəs.",     "пос. Рамана");
            AddS("Sabunçu",   "Kürdəxanı qəs.",  "пос. Кюрдаханы");
            AddS("Suraxanı",  "Suraxanı qəs.",   "пос. Сураханы");
            AddS("Suraxanı",  "Hövsan qəs.",     "пос. Гёвсан");
            AddS("Suraxanı",  "Lökbatan qəs.",   "пос. Локбатан");
            AddS("Qaradağ",   "Əhmədli qəs.",    "пос. Ахмедлы");
            AddS("Qaradağ",   "Müşviqabad qəs.", "пос. Мушвигабад");
            AddS("Qaradağ",   "Sahil qəs.",      "пос. Сахил");
            await context.Settlements.AddRangeAsync(settlements);
            await context.SaveChangesAsync();

            // ===== METRO STANSIYALARI =====
            var metros = new List<MetroStation>
            {
                new() { Name="İçərişəhər",        NameRu="Ичеришехер",        Line="Qırmızı",   LineColor="#E30613", SortOrder=1 },
                new() { Name="Sahil",              NameRu="Сахил",             Line="Qırmızı",   LineColor="#E30613", SortOrder=2 },
                new() { Name="28 May",             NameRu="28 Мая",            Line="Qırmızı",   LineColor="#E30613", SortOrder=3 },
                new() { Name="Gənclik",            NameRu="Гянджлик",          Line="Qırmızı",   LineColor="#E30613", SortOrder=4 },
                new() { Name="Nəriman Nərimanov",  NameRu="Нариман Нариманов", Line="Qırmızı",   LineColor="#E30613", SortOrder=5 },
                new() { Name="Bakmil",             NameRu="Бакмил",            Line="Qırmızı",   LineColor="#E30613", SortOrder=6 },
                new() { Name="Ulduz",              NameRu="Улдуз",             Line="Qırmızı",   LineColor="#E30613", SortOrder=7 },
                new() { Name="Koroğlu",            NameRu="Короглу",           Line="Qırmızı",   LineColor="#E30613", SortOrder=8 },
                new() { Name="Qara Qarayev",       NameRu="Кара Караев",       Line="Qırmızı",   LineColor="#E30613", SortOrder=9 },
                new() { Name="Neftçilər",          NameRu="Нефтчиляр",         Line="Qırmızı",   LineColor="#E30613", SortOrder=10 },
                new() { Name="Xalqlar Dostluğu",   NameRu="Халглар Достлугу",  Line="Qırmızı",   LineColor="#E30613", SortOrder=11 },
                new() { Name="Əhmədli",            NameRu="Ахмедлы",           Line="Qırmızı",   LineColor="#E30613", SortOrder=12 },
                new() { Name="Həzi Aslanov",       NameRu="Гязы Асланов",      Line="Qırmızı",   LineColor="#E30613", SortOrder=13 },
                new() { Name="Nəsimi",             NameRu="Насими",            Line="Yaşıl",     LineColor="#00A650", SortOrder=14 },
                new() { Name="Elmlər Akademiyası", NameRu="Академия Наук",     Line="Yaşıl",     LineColor="#00A650", SortOrder=15 },
                new() { Name="İnşaatçılar",        NameRu="Иншаатчылар",       Line="Yaşıl",     LineColor="#00A650", SortOrder=16 },
                new() { Name="20 Yanvar",          NameRu="20 Января",         Line="Yaşıl",     LineColor="#00A650", SortOrder=17 },
                new() { Name="Memar Əcəmi",        NameRu="Мемар Аджами",      Line="Yaşıl",     LineColor="#00A650", SortOrder=18 },
                new() { Name="Azadlıq Prospekti",  NameRu="Проспект Азадлыг",  Line="Yaşıl",     LineColor="#00A650", SortOrder=19 },
                new() { Name="Dərnəgül",           NameRu="Дарнагюль",         Line="Yaşıl",     LineColor="#00A650", SortOrder=20 },
                new() { Name="8 Noyabr",           NameRu="8 Ноября",          Line="Bənövşəyi", LineColor="#8E44AD", SortOrder=21 },
                new() { Name="Avtovağzal",         NameRu="Автовокзал",        Line="Bənövşəyi", LineColor="#8E44AD", SortOrder=22 },
            };
            await context.MetroStations.AddRangeAsync(metros);
            await context.SaveChangesAsync();

            // ===== MƏNZİL TİPLƏRİ =====
            var propTypes = new List<PropertyType>
            {
                new() { Name="Mənzil",     NameRu="Квартира",    NameEn="Apartment", Icon="🏠", SortOrder=1 },
                new() { Name="Həyət evi",  NameRu="Частный дом", NameEn="House",     Icon="🏡", SortOrder=2 },
                new() { Name="Bağ evi",    NameRu="Дача",        NameEn="Cottage",   Icon="🏘️", SortOrder=3 },
                new() { Name="Ofis",       NameRu="Офис",        NameEn="Office",    Icon="🏢", SortOrder=4 },
                new() { Name="Torpaq",     NameRu="Земля",       NameEn="Land",      Icon="🌿", SortOrder=5 },
                new() { Name="Kommersiya", NameRu="Коммерческое",NameEn="Commercial",Icon="🏬", SortOrder=6 },
            };
            await context.PropertyTypes.AddRangeAsync(propTypes);
            await context.SaveChangesAsync();

            // ===== BİNA NÖVLƏRİ =====
            var buildTypes = new List<BuildingType>
            {
                new() { Name="Yeni tikili",  NameRu="Новостройка",    NameEn="New building", HasProjects=false },
                new() { Name="Köhnə tikili", NameRu="Вторичное жильё",NameEn="Old building", HasProjects=true  },
            };
            await context.BuildingTypes.AddRangeAsync(buildTypes);
            await context.SaveChangesAsync();

            var oldBuildingId = (await context.BuildingTypes.FirstAsync(b => b.HasProjects)).Id;

            var projects = new List<BuildingProject>
            {
                new() { BuildingTypeId=oldBuildingId, Name="Xruşşovka", NameRu="Хрущёвка" },
                new() { BuildingTypeId=oldBuildingId, Name="Leninqrad", NameRu="Ленинградский проект" },
                new() { BuildingTypeId=oldBuildingId, Name="Kiyev",     NameRu="Киевский проект" },
                new() { BuildingTypeId=oldBuildingId, Name="Moskva",    NameRu="Московский проект" },
                new() { BuildingTypeId=oldBuildingId, Name="Stalinka",  NameRu="Сталинка" },
                new() { BuildingTypeId=oldBuildingId, Name="Panel",     NameRu="Панельный" },
                new() { BuildingTypeId=oldBuildingId, Name="Monolitik", NameRu="Монолитный" },
            };
            await context.BuildingProjects.AddRangeAsync(projects);
            await context.SaveChangesAsync();

            // ===== TƏMİR NÖVLƏRİ =====
            var repairs = new List<RepairType>
            {
                new() { Name="Təmirsiz",        NameRu="Без ремонта",         NameEn="Without repair" },
                new() { Name="Kosmetik təmir",  NameRu="Косметический ремонт",NameEn="Cosmetic repair" },
                new() { Name="Avropa təmiri",   NameRu="Евроремонт",          NameEn="Euro repair" },
                new() { Name="Dizayner təmiri", NameRu="Дизайнерский ремонт", NameEn="Designer repair" },
            };
            await context.RepairTypes.AddRangeAsync(repairs);
            await context.SaveChangesAsync();
        }

        private static async Task SeedDemoListingsAsync(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            if (context.Listings.Any()) return;

            var demoUsers = new[]
            {
                ("elvin.mammadov", "Elvin Məmmədov", "051-234-56-78"),
                ("gunel.aliyeva",  "Günəl Əliyeva",  "050-345-67-89"),
                ("tural.hasanov",  "Tural Həsənov",  "055-456-78-90"),
                ("aysel.rzayeva",  "Aysel Rzayeva",  "070-567-89-01"),
                ("kamran.jafarov", "Kamran Cəfərov",  "077-678-90-12"),
            };

            var userIds = new List<string>();
            foreach (var (uname, fullName, phone) in demoUsers)
            {
                var existing = await userManager.FindByNameAsync(uname);
                if (existing == null)
                {
                    var u = new IdentityUser { UserName = uname, EmailConfirmed = true };
                    var r = await userManager.CreateAsync(u, "Demo123!");
                    if (r.Succeeded) { await userManager.AddToRoleAsync(u, "User"); existing = u; }
                }
                if (existing != null) userIds.Add(existing.Id);
            }

            if (userIds.Count == 0) return;

            var bakuId   = (await context.Cities.FirstAsync(c => c.Name == "Bakı")).Id;
            var distMap  = await context.Districts.Where(d => d.CityId == bakuId).ToDictionaryAsync(d => d.Name, d => d.Id);
            var propMap  = await context.PropertyTypes.ToDictionaryAsync(p => p.Name, p => p.Id);
            var repMap   = await context.RepairTypes.ToDictionaryAsync(r => r.Name, r => r.Id);
            var btMap    = await context.BuildingTypes.ToDictionaryAsync(b => b.Name, b => b.Id);

            int D(string n) => distMap.GetValueOrDefault(n, 0);
            int P(string n) => propMap.GetValueOrDefault(n, 0);
            int R(string n) => repMap.GetValueOrDefault(n, 0);
            int B(string n) => btMap.GetValueOrDefault(n, 0);

            var now = DateTime.Now;
            var seed = new (string uname, string full, string phone, int cityId, int? dist, int prop, decimal pMin, decimal pMax, int? rMin, int? rMax, int? aMin, int? aMax, int? fMin, int? fMax, int? rep, int? bt, string? notes, int daysAgo)[]
            {
                ("elvin.mammadov","Elvin Məmmədov","051-234-56-78", bakuId, D("Yasamal"), P("Mənzil"),  85000,100000, 2,3, 65, 85, 3, 6, R("Avropa təmiri"), B("Yeni tikili"), "Metroya yaxın, rahat nəqliyyat", 0),
                ("elvin.mammadov","Elvin Məmmədov","051-234-56-78", bakuId, D("Nəsimi"),  P("Mənzil"),  120000,150000,3,4, 90,120, 5,10, R("Dizayner təmiri"),B("Yeni tikili"),"Şəhər mərkəzinə yaxın", 2),
                ("gunel.aliyeva", "Günəl Əliyeva", "050-345-67-89", bakuId, D("Nərimanov"),P("Mənzil"), 60000, 75000, 1,2, 45, 60, 2, 8, R("Kosmetik təmir"),B("Köhnə tikili"),"Sakit məhəllə, körpə üçün əlverişli",1),
                ("gunel.aliyeva", "Günəl Əliyeva", "050-345-67-89", bakuId, D("Xətai"),   P("Həyət evi"),180000,220000,4,5,150,200, null,null,R("Avropa təmiri"),B("Yeni tikili"), "Həyət-bağça, qaraj mövcuddur", 5),
                ("tural.hasanov", "Tural Həsənov", "055-456-78-90", bakuId, D("Nizami"),  P("Mənzil"),  95000,115000, 2,3, 70, 90, 4, 9, R("Avropa təmiri"), B("Yeni tikili"), "28 May metrosuna 10 dəqiqə", 3),
                ("tural.hasanov", "Tural Həsənov", "055-456-78-90", bakuId, D("Binagadi"), P("Mənzil"),  45000, 58000, 1,2, 38, 55, 1, 5, R("Təmirsiz"),      B("Köhnə tikili"),"Öz əlinlə etmək üçün", 7),
                ("aysel.rzayeva", "Aysel Rzayeva", "070-567-89-01", bakuId, D("Yasamal"), P("Mənzil"),  75000, 90000, 2,3, 60, 80, 3, 7, R("Kosmetik təmir"),B("Yeni tikili"), "Dağüstü parkına yaxın", 1),
                ("aysel.rzayeva", "Aysel Rzayeva", "070-567-89-01", bakuId, D("Sabail"),  P("Ofis"),   200000,250000,null,null,80,120,2,5,  R("Dizayner təmiri"),B("Yeni tikili"),"İş mərkəzinə çevirmək üçün ideal", 4),
                ("kamran.jafarov","Kamran Cəfərov","077-678-90-12", bakuId, D("Suraxanı"), P("Həyət evi"),90000,120000,3,4,120,160,null,null,R("Kosmetik təmir"),B("Köhnə tikili"),"Geniş həyət, 3 sot torpaq", 6),
                ("kamran.jafarov","Kamran Cəfərov","077-678-90-12", bakuId, D("Nəsimi"),  P("Mənzil"),  140000,170000,3,4,100,130,7,14,R("Dizayner təmiri"),B("Yeni tikili"), "Yüksək mərtəbə, panoram mənzərə", 2),
                ("elvin.mammadov","Elvin Məmmədov","051-234-56-78", bakuId, D("Xəzər"),   P("Bağ evi"), 55000, 70000, 2,3, 80,120,null,null,R("Kosmetik təmir"),B("Köhnə tikili"),"Dənizə 500 metr, sakit ərazi", 8),
                ("gunel.aliyeva", "Günəl Əliyeva", "050-345-67-89", bakuId, D("Sabunçu"), P("Mənzil"),  40000, 52000, 1,2, 40, 55, 2, 6, R("Təmirsiz"),      B("Köhnə tikili"),"Birinci mənzil deyil", 9),
            };

            var listings = new List<Listing>();
            int counter = 1;
            foreach (var s in seed)
            {
                var user = await userManager.FindByNameAsync(s.uname);
                if (user == null || s.cityId == 0 || s.prop == 0) continue;
                listings.Add(new Listing
                {
                    UserId          = user.Id,
                    ListingNumber   = $"EVT-{now.Year}-{counter++:D5}",
                    CityId          = s.cityId,
                    DistrictId      = s.dist > 0 ? s.dist : null,
                    PropertyTypeId  = s.prop,
                    BuildingTypeId  = s.bt > 0 ? s.bt : null,
                    RepairTypeId    = s.rep > 0 ? s.rep : null,
                    RoomMin         = s.rMin,
                    RoomMax         = s.rMax,
                    AreaMin         = s.aMin,
                    AreaMax         = s.aMax,
                    FloorMin        = s.fMin,
                    FloorMax        = s.fMax,
                    PriceMin        = s.pMin,
                    PriceMax        = s.pMax,
                    FullName        = s.full,
                    Phone           = s.phone,
                    Notes           = s.notes,
                    IsActive        = true,
                    IsFrozen        = false,
                    CreatedAt       = now.AddDays(-s.daysAgo),
                });
            }
            await context.Listings.AddRangeAsync(listings);
            await context.SaveChangesAsync();
        }
    }
}
