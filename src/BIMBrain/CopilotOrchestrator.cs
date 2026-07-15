namespace BIMBrain
{
    public class CopilotOrchestrator
    {
        private readonly CopilotContext _context;

        public CopilotOrchestrator(CopilotContext context)
        {
            _context = context;
        }

        public CopilotRoute Resolve()
        {
            return Resolve(_context);
        }

        public CopilotRoute Resolve(CopilotContext context)
        {
            if (context == null)
            {
                return new CopilotRoute
                {
                    RequestType = CopilotRequestType.Unknown,
                    EngineName = "Unknown",
                    Reason = "Contexto nulo.",
                    CanExecute = false
                };
            }

            if (context.HasQuestion)
            {
                var reason = context.HasSelection
                    ? "Existe pergunta e seleção; a pergunta tem prioridade e a IA utilizará a seleção posteriormente."
                    : "Existe pergunta e nenhuma seleção.";

                return new CopilotRoute
                {
                    RequestType = CopilotRequestType.Question,
                    EngineName = "Question Engine",
                    Reason = reason,
                    CanExecute = true
                };
            }

            if (context.HasSelection)
            {
                return new CopilotRoute
                {
                    RequestType = CopilotRequestType.Selection,
                    EngineName = "Selection Engine",
                    Reason = "Existe seleção e nenhuma pergunta.",
                    CanExecute = true
                };
            }

            return new CopilotRoute
            {
                RequestType = CopilotRequestType.Unknown,
                EngineName = "Unknown",
                Reason = "Não há pergunta nem seleção disponíveis.",
                CanExecute = false
            };
        }
    }
}
