using System;
using mPower.Domain.Application.Enums;
using mPower.Framework.Utils.Extensions;

namespace Default.Areas.Administration.Models
{
    public class SegmentOptionModel
    {   
        public bool Enabled { get; set; }

        public string Value { get; set; }

        public string Title { get; set; }

        public string Name { get; set; }

        public ComparerEnum Comparer { get; set; }

        public ConditionEnum Condition { get; set; }

        public FrequencyEnum Frequency { get; set; }

        public OptionTypeEnum Type { get; set; }

        public TrendEnum Trend { get; set; }

        public int Multiplier { get; set; }

        public string FormatString
        {
            get
            {
                switch (Type)
                {
                    case OptionTypeEnum.Flag:
                        return string.Format("{0}", Title);
                    case OptionTypeEnum.Trend:
                        return string.Format("{0} {1}", Title, Trend.GetDescription());
                    case OptionTypeEnum.Custom:
                        return string.Format("{0} {1}", Title, Value);
                    case OptionTypeEnum.Frequency:
                        return string.Format("{2} {0} {3} {1}", Title, Value, Frequency.GetDescription(), Comparer.GetDescription());
                    case OptionTypeEnum.Full:
                        return string.Format("{2} {0} {3} {1}", Title, Value, Condition.GetDescription(), Comparer.GetDescription());
                    default:
                        return string.Format("{0} {2} {1}", Title, Value, Comparer.GetDescription());
                }

                
            }
        }
        
        public SegmentOptionModel()
        {
            Frequency = FrequencyEnum.Month;
        }
    }
}