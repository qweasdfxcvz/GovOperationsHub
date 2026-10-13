using System.ComponentModel.DataAnnotations;


namespace GovOperationsHub.Domain.Enums
{
    public enum WorkflowStageStatus
    {
        [Display(Name = "لم تبدأ بعد")] NotStarted = 1,
        [Display(Name = "قيد التنفيذ")] InProgress = 2,
        [Display(Name = "بانتظار موافقة/إجراء الطرف الثاني")] PendingExternalAction = 3,
        [Display(Name = "تم الإنجاز بالوقت المحدد")] CompletedOnTime = 4,
        [Display(Name = "تم الإنجاز مع وجود تأخير")] CompletedDelayed = 5,
        [Display(Name = "متعثرة ومتجاوزة للمهلة (Breached SLA)")] Overdue = 6
    }
}
