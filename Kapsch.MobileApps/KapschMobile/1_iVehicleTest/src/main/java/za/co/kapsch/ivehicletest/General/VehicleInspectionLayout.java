package za.co.kapsch.ivehicletest.General;

import android.text.TextUtils;

import za.co.kapsch.ivehicletest.Enums.QuestionType;
import za.co.kapsch.ivehicletest.Enums.VehicleInspectionQuestionState;
import za.co.kapsch.ivehicletest.Models.VehicleInspectionQuestionModel;
import za.co.kapsch.ivehicletest.R;
import za.co.kapsch.shared.Enums.ErrorSeverity;
import za.co.kapsch.shared.MessageManager;

/**
 * Created by CSenekal on 2018/02/26.
 */

public class VehicleInspectionLayout {

//    private VehicleInspectionQuestionState getLayoutType(VehicleInspectionQuestionModel vehicleInspectionQuestion){
//
//        if (vehicleInspectionQuestion.isPhotoRequired() == true){
//            return VehicleInspectionQuestionState.Photo;
//        }
//
//        if ((vehicleInspectionQuestion.getQuestionType() == QuestionType.Text) &&
//            (vehicleInspectionQuestion.getIsDoubleEntry() == true) &&
//            (vehicleInspectionQuestion.getIsReadOnly() == false) &&
//            (TextUtils.isEmpty(vehicleInspectionQuestion.getValuePattern()) == true)) {
//            return VehicleInspectionQuestionState.DoubleEntryCompare;
//        }
//
//        if ((vehicleInspectionQuestion.getQuestionType() == QuestionType.Text) &&
//            (vehicleInspectionQuestion.getIsDoubleEntry() == true) &&
//            (vehicleInspectionQuestion.getIsReadOnly() == false) &&
//            (TextUtils.isEmpty(vehicleInspectionQuestion.getValuePattern()) == false)){
//            return VehicleInspectionQuestionState.DoubleEntryCompareValuePattern;
//        }
//
//        if (vehicleInspectionQuestion.getQuestionType() == QuestionType.MultipleChoice) {
//            return VehicleInspectionQuestionState.MultipleChoice;
//        }
//
//        if (vehicleInspectionQuestion.getQuestionType() == QuestionType.TextMultipleChoice){
//            return VehicleInspectionQuestionState.MultipleChoiceCompare;
//        }
//
//        if (vehicleInspectionQuestion.getQuestionType() == QuestionType.ChangeMultipleChoice){
//            return VehicleInspectionQuestionState.
//        }
//    }
}
