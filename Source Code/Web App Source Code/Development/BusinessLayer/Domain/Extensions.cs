using System;

using IID.BusinessLayer.Helpers;

namespace IID.BusinessLayer.Domain
{
    public partial class t_user
    {
        // NOTE: first_name and last_name members come from the database model.
        public string full_name { get { return (first_name + " " + last_name).Trim(); } }
    }

    public partial class t_observation_entry
    {
        private IndicatorType IndicatorType
        {
            get { return Enumerations.Parse<IndicatorType>(observation.indicator.indicator_type.value); }
        }

        public object indicator_value
        {
            get
            {
                switch (IndicatorType)
                {
                    case IndicatorType.Count:
                        return count;

                    case IndicatorType.Ratio:
                        return rate;

                    case IndicatorType.YesNo:
                        return yes_no;

                    default:
                        return ((denominator ?? 0) == 0 ? 0 : numerator * 1.0 / denominator);
                }

            }
        }

        public double? indicator_numeric_value
        {
            get
            {
                switch (IndicatorType)
                {
                    case IndicatorType.Count:
                        return count;

                    case IndicatorType.Ratio:
                        if (rate.HasValue)
                            return Convert.ToDouble(rate);
                        else
                            return null;

                    case IndicatorType.YesNo:
                        if (yes_no.HasValue)
                            return (yes_no.Value ? 1 : 0);
                        else
                            return null;

                    default:
                        if (numerator == null || denominator == null || denominator == 0)
                            return null;
                        else
                            return numerator * 1.0 / denominator;
                }
            }
        }

        public int numerator_or_value
        {
            get
            {
                switch (IndicatorType)
                {
                    case IndicatorType.Count:
                        return count.Value;

                    case IndicatorType.YesNo:
                        return yes_no.Value ? 1 : 0;

                    default:
                        return numerator.Value;
                }

            }
        }
    }

    public partial class t_activity : IRequestEntity
    {
        private string TranslationTable { get { return "t_activity"; } }
        private object[] TranslationKey { get { return new object[] { activity_id }; } }

        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "name", TranslationKey, languageId) ?? name;
        }

        public t_translation set_name_translated(byte languageId, string value, int userId)
        {
            if (name == null)
                name = value;
            return Globalization.SetTranslation(TranslationTable, "name", TranslationKey, languageId, value, userId);
        }

        public string get_other_key_information_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "other_key_information", TranslationKey, languageId) ?? other_key_information;
        }

        public t_translation set_other_key_information_translated(byte languageId, string value, int userId)
        {
            if (other_key_information == null)
                other_key_information = value;
            return Globalization.SetTranslation(TranslationTable, "other_key_information", TranslationKey, languageId, value, userId);
        }
    }

    //public static class Extensions
    //{
    //    public static string get_name_translated(this t_activity entity, byte languageId)
    //    {
    //        return Globalization.GetTranslation(entity, "name", entity.activity_id, languageId) ?? entity.name;
    //    }

    //    public static string get_other_key_information_translated(this t_activity entity, byte languageId)
    //    {
    //        return Globalization.GetTranslation(entity, "other_key_information", entity.activity_id, languageId) ?? entity.other_key_information;
    //    }
    //}

    public partial class t_administrative_division
    {
        private string TranslationTable { get { return "t_administrative_division"; } }
        private object[] TranslationKey { get { return new object[] { administrative_division_id }; } }

        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "name", TranslationKey, languageId) ?? name;
        }

        public t_translation set_name_translated(byte languageId, string value, int userId)
        {
            if (name == null)
                name = value;
            return Globalization.SetTranslation(TranslationTable, "name", TranslationKey, languageId, value, userId);
        }
    }

    public partial class t_aim : IRequestEntity
    {
        private string TranslationTable { get { return "t_aim"; } }
        private object[] TranslationKey { get { return new object[] { aim_id }; } }

        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "name", TranslationKey, languageId) ?? name;
        }

        public t_translation set_name_translated(byte languageId, string value, int userId)
        {
            if (name == null)
                name = value;
            return Globalization.SetTranslation(TranslationTable, "name", TranslationKey, languageId, value, userId);
        }
    }

    public partial class t_fieldid
    {
        private string TranslationTable { get { return "t_fieldid"; } }
        private object[] TranslationKey { get { return new object[] { fieldid }; } }

        public string get_value_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "value", TranslationKey, languageId) ?? value;
        }

        public t_translation set_value_translated(byte languageId, string value, int userId)
        {
            if (this.value == null)
                this.value = value;
            return Globalization.SetTranslation(TranslationTable, "value", TranslationKey, languageId, value, userId);
        }
    }

    public partial class t_project : IRequestEntity
    {
        private string TranslationTable { get { return "t_project"; } }
        private object[] TranslationKey { get { return new object[] { project_id }; } }

        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "name", TranslationKey, languageId) ?? name;
        }

        public t_translation set_name_translated(byte languageId, string value, int userId)
        {
            if (name == null)
                name = value;
            return Globalization.SetTranslation(TranslationTable, "name", TranslationKey, languageId, value, userId);
        }
    }

    public partial class t_country : IRequestEntity
    {
        private string TranslationTable { get { return "t_country"; } }
        private object[] TranslationKey { get { return new object[] { country_id }; } }

        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "name", TranslationKey, languageId) ?? name;
        }

        public t_translation set_name_translated(byte languageId, string value, int userId)
        {
            if (name == null)
                name = value;
            return Globalization.SetTranslation(TranslationTable, "name", TranslationKey, languageId, value, userId);
        }

        public string get_first_level_administrative_division_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "first_level_administrative_division", TranslationKey, languageId) ?? first_level_administrative_division;
        }

        public t_translation set_first_level_administrative_division_translated(byte languageId, string value, int userId)
        {
            if (first_level_administrative_division == null)
                first_level_administrative_division = value;
            return Globalization.SetTranslation(TranslationTable, "first_level_administrative_division", TranslationKey, languageId, value, userId);
        }

        public string get_second_level_administrative_division_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "second_level_administrative_division", TranslationKey, languageId) ?? second_level_administrative_division;
        }

        public t_translation set_second_level_administrative_division_translated(byte languageId, string value, int userId)
        {
            if (second_level_administrative_division == null)
                second_level_administrative_division = value;
            return Globalization.SetTranslation(TranslationTable, "second_level_administrative_division", TranslationKey, languageId, value, userId);
        }

        public string get_third_level_administrative_division_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "third_level_administrative_division", TranslationKey, languageId) ?? third_level_administrative_division;
        }

        public t_translation set_third_level_administrative_division_translated(byte languageId, string value, int userId)
        {
            if (third_level_administrative_division == null)
                third_level_administrative_division = value;
            return Globalization.SetTranslation(TranslationTable, "third_level_administrative_division", TranslationKey, languageId, value, userId);
        }

        public string get_fourth_level_administrative_division_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "fourth_level_administrative_division", TranslationKey, languageId) ?? fourth_level_administrative_division;
        }

        public t_translation set_fourth_level_administrative_division_translated(byte languageId, string value, int userId)
        {
            if (fourth_level_administrative_division == null)
                fourth_level_administrative_division = value;
            return Globalization.SetTranslation(TranslationTable, "fourth_level_administrative_division", TranslationKey, languageId, value, userId);
        }
    }

    public partial class t_indicator : IRequestEntity
    {
        private string TranslationTable { get { return "t_indicator"; } }
        private object[] TranslationKey { get { return new object[] { indicator_id }; } }

        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "name", TranslationKey, languageId) ?? name;
        }

        public t_translation set_name_translated(byte languageId, string value, int userId)
        {
            if (name == null)
                name = value;
            return Globalization.SetTranslation(TranslationTable, "name", TranslationKey, languageId, value, userId);
        }

        public string get_definition_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "definition", TranslationKey, languageId) ?? definition;
        }

        public t_translation set_definition_translated(byte languageId, string value, int userId)
        {
            if (definition == null)
                definition = value;
            return Globalization.SetTranslation(TranslationTable, "definition", TranslationKey, languageId, value, userId);
        }

        public string get_numerator_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "numerator_name", TranslationKey, languageId) ?? numerator_name;
        }

        public t_translation set_numerator_name_translated(byte languageId, string value, int userId)
        {
            if (numerator_name == null)
                numerator_name = value;
            return Globalization.SetTranslation(TranslationTable, "numerator_name", TranslationKey, languageId, value, userId);
        }

        public string get_numerator_definition_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "numerator_definition", TranslationKey, languageId) ?? numerator_definition;
        }

        public t_translation set_numerator_definition_translated(byte languageId, string value, int userId)
        {
            if (numerator_definition == null)
                numerator_definition = value;
            return Globalization.SetTranslation(TranslationTable, "numerator_definition", TranslationKey, languageId, value, userId);
        }

        public string get_numerator_source_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "numerator_source", TranslationKey, languageId) ?? numerator_source;
        }

        public t_translation set_numerator_source_translated(byte languageId, string value, int userId)
        {
            if (numerator_source == null)
                numerator_source = value;
            return Globalization.SetTranslation(TranslationTable, "numerator_source", TranslationKey, languageId, value, userId);
        }

        public string get_denominator_name_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "denominator_name", TranslationKey, languageId) ?? denominator_name;
        }

        public t_translation set_denominator_name_translated(byte languageId, string value, int userId)
        {
            if (denominator_name == null)
                denominator_name = value;
            return Globalization.SetTranslation(TranslationTable, "denominator_name", TranslationKey, languageId, value, userId);
        }

        public string get_denominator_definition_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "denominator_definition", TranslationKey, languageId) ?? denominator_definition;
        }

        public t_translation set_denominator_definition_translated(byte languageId, string value, int userId)
        {
            if (denominator_definition == null)
                denominator_definition = value;
            return Globalization.SetTranslation(TranslationTable, "denominator_definition", TranslationKey, languageId, value, userId);
        }

        public string get_denominator_source_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "denominator_source", TranslationKey, languageId) ?? denominator_source;
        }

        public string get_data_frequency_translated(byte languageId, string TranslationKey)
        {
            object[] Key = new object[] { TranslationKey };
            return Globalization.GetTranslation("t_fieldid", "value", Key, languageId) ?? TranslationKey;
        }

        public t_translation set_denominator_source_translated(byte languageId, string value, int userId)
        {
            if (denominator_source == null)
                denominator_source = value;
            return Globalization.SetTranslation(TranslationTable, "denominator_source", TranslationKey, languageId, value, userId);
        }
    }

    public partial class t_site : IRequestEntity
    {
        private string TranslationTable { get { return "t_site"; } }
        private object[] TranslationKey { get { return new object[] { site_id }; } }

        public string get_other_key_information_translated(byte languageId)
        {
            return Globalization.GetTranslation(TranslationTable, "other_key_information", TranslationKey, languageId) ?? other_key_information;
        }

        public t_translation set_other_key_information_translated(byte languageId, string value, int userId)
        {
            if (other_key_information == null)
                other_key_information = value;
            return Globalization.SetTranslation(TranslationTable, "other_key_information", TranslationKey, languageId, value, userId);
        }
    }

    public partial class v_site
    {
        public string get_name_translated(byte languageId)
        {
            return Globalization.GetTranslation("t_site", "name", new object[] { site_id }, languageId) ?? name;
        }

        public string get_country_name_translated(byte languageId)
        {
            return Globalization.GetTranslation("t_country", "name", new object[] { country_id }, languageId) ?? name;
        }
    }

    public partial class t_activity_additional_manager
    {
        public int activity_id { get; set; }
        public int additional_manager_userid { get; set; }
    }

    public partial class t_activity_technical_area_subtype
    {
        public int activity_id { get; set; }
        public string technical_area_subtype_fieldid { get; set; }
    }

    public partial class t_user :  IRequestEntity
    {

    }

    public partial class t_observation_attachment : IRequestEntity
    {

    }

    public interface IRequestEntity
    {
        int createdby_userid { get; set; }
        t_user createdby { get; set; }
        DateTime created_date { get; set; }
        int? updatedby_userid { get; set; }
        t_user updatedby { get; set; }
        DateTime? updated_date { get; set; }
        bool? active { get; set; }
    }
}
