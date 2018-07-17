package za.co.kapsch.ivehicletest.Enums;

import com.google.gson.annotations.SerializedName;

/**
 * Created by csenekal on 2017/12/04.
 */

public enum QuestionType {

    //eg. VIN, CHASSIS, ENG, REGISTRATION: User enters value twice and value must be compared to backend value - (also validate against regular expression valuePattern if exists)
    //IsCompared        in QuestionModel
    //IsDoubleEntry     in QuestionModel
    //IsReadOnly        in QuestionModel
    //HasComment        in QuestionModel
    //ValuePattern      in QuestionModel
    //IsCommentRequired in AnswerModel
    @SerializedName("1")
    Text(1),

    //eg. Wheels and tyres? Response is multiple choice eg. Pass, Fail
    //IsReadOnly in QuestionModel
    //IsCommentRequired in AnswerModel
    @SerializedName("2")
    MultipleChoice(2),

    //eg. Question: Colour - User receives a compare value eg. Red + multiple choice eg. Pass, Fail
    @SerializedName("3")
    TextMultipleChoice(3),

    /*Ammend eg. VIN or Chassis or VIN or ENG or Colour GVM (only one can be ammended)
    Value change eg. Chassis, VIN, ENG, Colour, GVM - fail, change - Comment required on fail (correct value 3)
    OR
    Ammend Other eg. multiple selection pass or change both are regarded as correct
    Pass, Change (correct value 1,3)*/
    @SerializedName("4")
    ChangeMultipleChoice(4);

    private final int code;

    QuestionType(int code) {
        this.code = code;
    }

    public int getCode() {
        return this.code;
    }
}

