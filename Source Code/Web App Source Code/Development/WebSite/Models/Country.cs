using IID.BusinessLayer.Domain;
using IID.WebSite.Helpers;

namespace IID.WebSite.Models
{
    public class Country
    {
        public Country() { }

        public Country(int countryId)
        {
            using (Entity context = new Entity())
            {
                t_country entity = context.t_country.Find(countryId);
                SetProperties(entity, true);
            }
        }

        public Country(t_country entity)
        {
            SetProperties(entity, false);
        }

        private void SetProperties(t_country entity, bool translate)
        {
            CountryId = entity.country_id;
            Active = entity.active;
            ParentSiteTyepFieldId = entity.parent_site_type_fieldid;

            if (translate)
            {
                byte languageId = IidCulture.CurrentLanguageId;
                Name = entity.get_name_translated(languageId);
                FirstLevelAdministrativeDivision = entity.get_first_level_administrative_division_translated(languageId);
                SecondLevelAdministrativeDivision = entity.get_second_level_administrative_division_translated(languageId);
                ThirdLevelAdministrativeDivision = entity.get_third_level_administrative_division_translated(languageId);
                FourthLevelAdministrativeDivision = entity.get_fourth_level_administrative_division_translated(languageId);
            }
            else
            {
                Name = entity.name;
                FirstLevelAdministrativeDivision = entity.first_level_administrative_division;
                SecondLevelAdministrativeDivision = entity.second_level_administrative_division;
                ThirdLevelAdministrativeDivision = entity.third_level_administrative_division;
                FourthLevelAdministrativeDivision = entity.fourth_level_administrative_division;
            }
        }

        public int CountryId { get; set; }
        public string Name { get; set; }
        public bool? Active{ get; set; }
        public string ParentSiteTyepFieldId { get; set; }
        public string FirstLevelAdministrativeDivision { get; set; }
        public string SecondLevelAdministrativeDivision { get; set; }
        public string ThirdLevelAdministrativeDivision { get; set; }
        public string FourthLevelAdministrativeDivision { get; set; }
    }
}