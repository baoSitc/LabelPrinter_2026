using System;

namespace LabelPrinter.Services
{
    public static class LabelServiceFactory
    {
        public static ILabelService Create(
            string? labelSizeId)
        {
            return labelSizeId switch
            {
                "A7" =>
                    new LabelServiceA7(),

                "50x50" =>
                    new LabelService50x50(),

                "50x30" =>
                    new LabelService50x30(),

                "75x50" =>
                    new LabelService75x50(),

                "100x150" =>
                    new LabelService100x150(),

                _ =>
                    throw new ArgumentException(
                        $"Không hỗ trợ khổ tem: {labelSizeId}")
            };
        }
    }
}