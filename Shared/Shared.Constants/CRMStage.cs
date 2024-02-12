
namespace Shared.Constants
{
    public class CRMStage
    {
        public static string NEW = "New";

        public static string ATTEMPTED_1 = "Attempted 1";

        public static string ATTEMPTED_2 = "Attempted 2";

        public static string ATTEMPTED_3 = "Attempted 3";

        public static string ATTEMPTED_4 = "Attempted 4";

        public static string ATTEMPTED_5 = "Attempted 5";

        public static string ATTEMPTED_6 = "Attempted 6";

        public static string Attempt_7_No_Contact_Made = "Attempt 7 - No Contact Made";

        public static string RETURNING_NEW = "Returning - New";

        public static string ON_CALL_NOW = "On call now";

        public static string CALL_BACK_BOOKED = "Call Back Booked";

        public static string CALL_BACK_BOOKED_ATTEMPT_1 = "Call Back Booked Attempt 1";

        public static string CONTACT_IN_FUTURE = "Contact in Future";

        public static string CONTACT_IN_FUTURE_ATTEMPT_1 = "Contact in Future Attempt 1";

        public static string CONTACT_IN_FUTURE_ATTEMPT_2 = "Contact in Future Attempt 2";

        public static string CONTACT_IN_FUTURE_QUALIFIED = "Contact in Future Qualified";

        public static string CONTACT_IN_FUTURE_U_A = "Contact in Future U/A";

        public static string RETURNING_QUOTE = "Returning - Quote";

        public static string QUOTE = "Quote";

        public static string QUOTE_ATTEMPT_1 = "Quote Attempt 1";

        public static string QUOTE_ATTEMPT_2 = "Quote Attempt 2";

        public static string QUOTE_ATTEMPT_3 = "Quote Attempt 3";

        public static string QUOTE_ATTEMPT_4 = "Quote Attempt 4";

        public static string QUOTE_ATTEMPTED_U_A = "Quote Attempted U/A";

        public static string RETURNING_APPLICATION = "Returning - Application";

        public static string APPLICATION = "Application";

        public static string WRONG_NUMBER = "Wrong Number";

        public static string NOT_INTERESTED = "Not Interested";

        public static string NO_ANSWER = "No Answer";

        public static string ADVISED_TO_STAY = "Advised to Stay";

        public static string DIRECT_TO_FUND = "Direct to fund";

        public static string INELIGIBLE = "Ineligible";
        
        public static string EMAIL_ONLY = "Email only";
        
        public static string DUPLICATE_LEAD = "Duplicate lead";

        public static string GetNexStage(string currentStage)
        {
            var nextStage = currentStage switch
            {
                "New" => ATTEMPTED_1,
                "Attempted 1" => ATTEMPTED_2,
                "Attempted 2" => ATTEMPTED_3,
                "Attempted 3" => ATTEMPTED_4,
                "Attempted 4" => ATTEMPTED_5,
                "Attempted 5" => ATTEMPTED_6,
                "Attempted 6" => Attempt_7_No_Contact_Made,
                "Returning - New" => ATTEMPTED_1,
                "Call Back Booked" => "",
                "Call Back Booked - Attempted" => "",
                "Contact in Future" => CONTACT_IN_FUTURE_ATTEMPT_1,
                "Contact in Future Attempt 1" => CONTACT_IN_FUTURE_ATTEMPT_2,
                "Contact in Future Attempt 2" => CONTACT_IN_FUTURE_U_A,
                "Contact in Future Qualified" => CONTACT_IN_FUTURE_ATTEMPT_1,
                "Online Quote" => ATTEMPTED_1,
                "Returning - Quote" => ATTEMPTED_1,
                "Quote" => QUOTE_ATTEMPT_1,
                "Quote Attempt 1" => QUOTE_ATTEMPT_2,
                "Quote Attempt 2" => QUOTE_ATTEMPT_3,
                "Quote Attempt 3" => QUOTE_ATTEMPT_4,
                "Quote Attempt 4" => QUOTE_ATTEMPTED_U_A,
                "Returning - Application" => QUOTE_ATTEMPT_1,
                "Application" => QUOTE_ATTEMPT_1,
                _ => ""
            };

            return nextStage;
        }
    }
}
