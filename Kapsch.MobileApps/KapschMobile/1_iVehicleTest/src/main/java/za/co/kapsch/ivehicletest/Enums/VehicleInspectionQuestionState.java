package za.co.kapsch.ivehicletest.Enums;

/**
 * Created by CSenekal on 2018/02/26.
 */

public enum VehicleInspectionQuestionState {

    //
    Photo,

    //User enters value, value is compared to backend value to set pass or failed
    Text,

    //User enters value twice and value is compared to backend value
    DoubleEntryCompare,

    //User enters value twice, value is compared to backend value and also validate against regular expression in from backend
    DoubleEntryCompareValuePattern,

    //eg. Question: Wheels and tyres? Response is multiple choice eg. Pass, Fail
    MultipleChoice,

    //eg. Question: Colour - User receives a compare value eg. Red + multiple choice eg. Pass, Fail
    MultipleChoiceCompare,

    //Ammend eg. VIN or Chassis or ENG or Colour GVM (only one can be ammended)
    //Value change eg. Chassis, VIN, ENG, Colour, GVM - fail, change - Comment required on fail (correct value 3)
    ChangeValueRequiredMultipleChoice,

    //Ammend Other eg. multiple selection pass or change both is regarded as correct
    //Pass, Change (correct value 1,3)
    ChangeValueOptionalMultipleChoice,
}
