namespace GenieOwl.Utilities.Prompts
{
    using GenieOwl.Utilities.Types;
    
    public static class Prompts
    {
        private static readonly Dictionary<PromptType, string> _PromptsEnglish = new()
        {
            { PromptType.SystemRole, "You'll respond as GenieOwl in an informal tone." },
            { PromptType.AchievementGuide, "Search online for: 'Guide on how to obtain the achievement {0} in {1}. " +
                "Analyze the results and create a descriptive but not overly detailed summary of the steps required to achieve the achievement. " +
                "If you need additional information such as the game's genre, languages, or platforms to complete the summary, you can search online for: '(NecessaryInformation) {1}'. " +
                "If you believe the summary may contain spoilers (details revealing the plot) of the game, mention it at the beginning." +
                "At the end, list the steps to follow in chronological order." +
                "Example of a descriptive but not overly detailed summary: " +
                "The achievement Maximum Difficulty in AnyGame can be obtained in the final part of Act 3. (May contain spoilers). " +
                "The first step occurs when you reach the Astral realm. You can choose to betray the Emperor and free Orpheus, or stay with the Emperor and not free Orpheus. " +
                "Either way, the dialogue will reach a point where someone can turn the parasite into an illicit to defeat the Netherbrain. There will be an option for you to sacrifice yourself and become an illithid; you must choose this. " +
                "(*Note: if you have Karlach in your group, she will try to take the parasite multiple times. Make sure to always select the option where you reject this)." +
                "Then you will transform into an illithid and must complete all final battle sequences as your new illithid self to achieve the achievement (both the courtyard sections and the final boss fight after scaling the brainstem)." +
                "Once you defeat the Netherbrain, you will be taken to Baldur's Gate for the final scenes and dialogue options. If you plan to enjoy the story and epilogue, choose the dialogue options you desire." +
                "1. Reach Act 3 of the game." +
                "2. Defeat Gortash and Oran." +
                "3. Reach the Netherbrain, you'll be sent to the Astral realm." +
                "4. Here you'll be given the option to become an illithid, you must select this option." +
                "5. You must continue the plot and finish the game as an illithid." }
        };

        private static readonly Dictionary<PromptType, string> _PromptsSpanish = new()
        {
            { PromptType.SystemRole, "Te llamarás GenieOwl y responderás bajo un tono informal." },
            { PromptType.AchievementGuide, "Busca en línea: 'Guía sobre cómo obtener el logro {0} en {1}. " +
                "Analiza los resultados y crea un resumen descriptivo pero no excesivamente detallado de los pasos necesarios para lograr el logro. " +
                "Si necesitas información adicional como el género del juego, idiomas o plataformas para completar el resumen, puedes buscar en línea: '(InformaciónNecesaria) {1}'. " +
                "Si crees que el resumen puede contener spoilers (detalles que revelan la trama) del juego, menciónalo al principio." +
                "Al final, enumera los pasos a seguir en orden cronológico." +
                "Ejemplo de un resumen descriptivo pero no excesivamente detallado: " +
                "El logro Dificultad Máxima en AnyGame se puede obtener en la parte final del Acto 3. (Puede contener spoilers). " +
                "El primer paso ocurre cuando alcanzas el reino astral. Puedes elegir traicionar al Emperador y liberar a Orpheus, o quedarte con el Emperador y no liberar a Orpheus. " +
                "De cualquier manera, el diálogo llegará a un punto en el que alguien puede convertir al parásito en un ilícito para derrotar al Cerebro Infernal. Habrá una opción para que te sacrifiques y te conviertas en un illithid; debes elegir esto. " +
                "(*Nota: si tienes a Karlach en tu grupo, intentará tomar el parásito varias veces. Asegúrate de siempre seleccionar la opción donde rechazas esto)." +
                "Luego te transformarás en un illithid y deberás completar todas las secuencias de batalla finales como tu nuevo yo illithid para lograr el logro (tanto las secciones del patio como la pelea final contra el cerebro después de escalar el tallo cerebral)." +
                "Una vez que derrotes al Cerebro Infernal, serás llevado a Baldur's Gate para las escenas y opciones de diálogo finales. Si planeas disfrutar de la historia y el epílogo, elige las opciones de diálogo que desees." +
                "1. Alcanza el Acto 3 del juego." +
                "2. Derrota a Gortash y Oran." +
                "3. Alcanza al Cerebro Infernal, serás enviado al reino astral." +
                "4. Aquí se te dará la opción de convertirte en un illithid, debes seleccionar esta opción." +
                "5. Debes continuar la trama y terminar el juego como un illithid." }
        };

        public static string GetPrompt(LenguageType lenguage, PromptType promptType) => GetLenguage(lenguage)[promptType];

        public static string GetPrompt(LenguageType lenguage, PromptType promptType, string parameter) => String.Format(GetLenguage(lenguage)[promptType], parameter);

        public static string GetPrompt(LenguageType lenguage, PromptType promptType, string[] parameters) => String.Format(GetLenguage(lenguage)[promptType], parameters); 

        private static Dictionary<PromptType, string> GetLenguage(LenguageType lenguage) => lenguage == LenguageType.English ? _PromptsEnglish : _PromptsSpanish;
    }
}
