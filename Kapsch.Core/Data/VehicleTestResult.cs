using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Kapsch.Core.Data
{
    [Table("VEHICLE_TEST_RESULT", Schema = "TIS")]
    public class VehicleTestResult
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column("VEHICLE_TEST_BOOKING_ID")]
        public int VehicleTestBookingID { get; set; }


        [Column("TEST_TYPE_ID")]
        public int TestTypeID { get; set; }


        [Column("TEST_QUESTIONS_ID")]
        public int TestQuestionsID { get; set; }


        [Column("TEXT_ANSWER")]
        public string TextAnswer { get; set; }


        [Column("TEST_QUESTIONS_ANSWERS_ID")]
        public Nullable<int> TestQuestionsAnswersID { get; set; }


        [Column("TEST_QUESTIONS_ANSWERS_REL_ID")]
        public int TestQuestionsAnswersRelID { get; set; }


        [Column("COMMENTS")]
        public string Comments { get; set; }

        [ForeignKey("TestQuestionsID")]
        public virtual VehicleTestQuestion VehicleTestQuestion { get; set; }

        [ForeignKey("TestQuestionsAnswersID")]
        public virtual VehicleTestQuestionAnswer VehicleTestQuestionAnswer { get; set; }

        [ForeignKey("VehicleTestBookingID")]
        public virtual Kapsch.Core.Data.VehicleTestBooking VehicleTestBooking { get; set; }

    }
}
