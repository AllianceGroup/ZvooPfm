using System.Collections.Generic;
using System.Linq;
using com.yodlee.common;
using com.yodlee.core.mfarefresh;

namespace mPower.Domain.Yodlee.Form
{
    public static class FormHelpers
    {
        public static void BuildComponents(ref List<FormInputGroup> formInputGroups, object[] fieldInfos,
                                           ref FormInputGroup parentFormInputGroup)
        {
            foreach (var component in fieldInfos)
            {
                var parentInputGroupPassedIn = parentFormInputGroup != null ? true : false;
                var formInputGroup = new FormInputGroup
                                         {
                                             FormInputs = new List<FormInput>(),
                                             FormInputGroups = new List<FormInputGroup>()
                                         };
                if (component is FieldInfoSingle)
                {
                    var fieldInfoSingle = (FieldInfoSingle) component;
                    var x = BuildFormInput(fieldInfoSingle.displayName,
                                           fieldInfoSingle.fieldType != null
                                               ? fieldInfoSingle.fieldType.typeName
                                               : "UNKNOWN",
                                           fieldInfoSingle.valueIdentifier,
                                           fieldInfoSingle.value,
                                           fieldInfoSingle.validValues,
                                           fieldInfoSingle.displayValidValues);
                    formInputGroup.FormInputs.Add(x);
                    formInputGroup.Layout = LayoutType.Standard;
                    if (parentInputGroupPassedIn)
                    {
                        parentFormInputGroup.FormInputGroups.Add(formInputGroup);
                    }
                    else
                    {
                        formInputGroups.Add(formInputGroup);
                    }
                }
                else if (component is FieldInfoMultiFixed)
                {
                    var fieldInfoMultiFixed = (FieldInfoMultiFixed) component;
                    formInputGroup.FormInputs.AddRange(
                        fieldInfoMultiFixed.valueIdentifiers.Select(
                            (t, i) =>
                            BuildFormInput(fieldInfoMultiFixed.displayName, fieldInfoMultiFixed.fieldTypes[i].typeName,
                                           t, fieldInfoMultiFixed.values[i], fieldInfoMultiFixed.validValues[i],
                                           fieldInfoMultiFixed.displayValidValues[i])));
                    formInputGroup.Layout = LayoutType.LeftToRight;
                    if (parentInputGroupPassedIn)
                    {
                        parentFormInputGroup.FormInputGroups.Add(formInputGroup);
                    }
                    else
                    {
                        formInputGroups.Add(formInputGroup);
                    }
                }
                else if (component is FieldInfoChoice)
                {
                    var fieldInfoChoice = (FieldInfoChoice) component;
                    formInputGroup.Layout = LayoutType.OptionalGroupings;
                    BuildComponents(ref formInputGroups, fieldInfoChoice.fieldInfoList, ref formInputGroup);
                    formInputGroups.Add(formInputGroup);
                }

				
            }
        }

        public static FormInput BuildFormInput(string label, string typeName, string valueIdentifier, string value,
                                               string[] validValues, string[] displayValidValues)
        {
            const string IF_LOGIN = "IF_LOGIN";
            const string IF_TEXT = "TEXT";
            const string IF_PASSWORD = "IF_PASSWORD";
            const string IF_OPTIONS = "OPTIONS";
            const string IF_RADIO = "RADIO";
            const string IF_UNKNOWN = "UNKNOWN";
            if (IF_TEXT.Equals(typeName) || IF_LOGIN.Equals(typeName))
            {
                var input = new FormInput
                                {
                                    Label = label,
                                    Type = "Text",
                                    Name = valueIdentifier,
                                    Value = value
                                };
                return input;
            }
            if (IF_PASSWORD.Equals(typeName))
            {
                var input = new FormInput
                                {
                                    Label = label,
                                    Type = "Password",
                                    Name = valueIdentifier,
                                    Value = value
                                };
                return input;
            }
            if (IF_OPTIONS.Equals(typeName) || IF_RADIO.Equals(typeName))
            {
                var input = new FormInput
                                {
                                    Label = label,
                                    Type = "Select",
                                    Name = valueIdentifier,
                                    Value = value,
                                    SelectOptions = new List<SelectOption>()
                                };
                if (validValues != null)
                {
                    for (var i = 0; i < validValues.Length; i++)
                    {
                        input.SelectOptions.Add(new SelectOption
                                                    {
                                                        Name = displayValidValues[i],
                                                        Value = validValues[i]
                                                    });
                    }
                }
                return input;
            }
            if (IF_UNKNOWN.Equals(typeName))
            {
                var input = new FormInput
                                {
                                    Label = label,
                                    Type = "UNKNOWN",
                                    Name = valueIdentifier,
                                    Value = value
                                };
                return input;
            }
            return null;
        }

        /// <summary>
        /// Fills out a Yodlee FormInput Object from the dictionary coming from post.
        /// Basically this searches the form input object for the same name value and replaces it's value
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="parameters"></param>
        public static void BuildParameterList(ref object[] fieldInfo, Dictionary<string, object> parameters)
        {
            var formFieldInfoList = new List<object>();

            foreach (var field in fieldInfo)
            {
                if (field is FieldInfoSingle)
                {
                    var fieldInfoSingle = (FieldInfoSingle) field;
                    fieldInfoSingle.value = parameters[fieldInfoSingle.valueIdentifier].ToString();
                    formFieldInfoList.Add(fieldInfoSingle);
                }
                else if (field is FieldInfoMultiFixed)
                {
                    var fieldInfoMultiFixed = (FieldInfoMultiFixed) field;
                    for (var i = 0; i < fieldInfoMultiFixed.valueIdentifiers.Length; i++)
                    {
                        fieldInfoMultiFixed.values[i] = parameters[fieldInfoMultiFixed.valueIdentifiers[i]].ToString();
                    }
                    formFieldInfoList.Add(fieldInfoMultiFixed);
                }
                else if (field is FieldInfoChoice)
                {
                    var fieldInfoChoice = (FieldInfoChoice) field;
                    BuildParameterList(ref fieldInfoChoice.fieldInfoList, parameters);
                    formFieldInfoList.Add(fieldInfoChoice);
                }
            }
        }


        public static FormInputGroup BuildQuestionAnswers(QuestionAndAnswerValues[] questionAndAnswerValues)
        {
            var formInputGroup = new FormInputGroup(){FormInputs =  new List<FormInput>()};
            
            foreach (var data in questionAndAnswerValues.OfType<SingleQuesSingleAnswerValues>())
            {
                formInputGroup.FormInputs.Add(new FormInput()
                                                  {
                                                      Label = data.question,
                                                      Type = data.responseFieldType,
                                                      Name = data.sequence.ToString()
                                                  });
            }


            return formInputGroup;
        }

        public static MFAUserResponse BuildQuestionAnswersInput(List<FormInput> input)
        {

           var quesAnsDetails = new List<QuesAndAnswerDetails>();

            foreach (var i in input)
            {
                var quesAnsDetail = new QuesAndAnswerDetails()
                                        {

                                            answer = i.Value,
                                            question = i.Label,
                                            answerFieldType = i.Type,
                                            questionFieldType = "label"

                                        };

                quesAnsDetails.Add(quesAnsDetail);
            }

            var mfaQuesAnsResponse = new MFAQuesAnsResponse();
            mfaQuesAnsResponse.quesAnsDetailArray = quesAnsDetails.ToArray();
            
            MFAUserResponse mfaUserResponse = mfaQuesAnsResponse;

            return mfaUserResponse;
        }
    }
}