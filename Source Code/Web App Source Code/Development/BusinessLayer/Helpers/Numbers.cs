using System.Globalization;

namespace IID.BusinessLayer.Helpers
{
    public static class Numbers
    {
        private static NumberFormatInfo _percentWithoutSpace = new NumberFormatInfo() { PercentPositivePattern = 1 };
        public static NumberFormatInfo PercentWithoutSpace { get { return _percentWithoutSpace; } }
    }
}
