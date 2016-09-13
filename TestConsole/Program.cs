﻿using CognitiveBot.BusinessLogic;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //var input ="(TOP (INTJ (UH Welcome) (PP (TO to) (NP (NNP Microsoft) (NNP Linguistic) (NNP Analysis))) (. !)))";
            var inputText = @"Anna Pávlovna's drawing room was gradually filling. The highest
Petersburg society was assembled there: people differing widely in age
and character but alike in the social circle to which they belonged.
Prince Vasíli's daughter, the beautiful Hélène, came to take her father
to the ambassador's entertainment; she wore a ball dress and her badge
as maid of honor. The youthful little Princess Bolkónskaya, known as la
femme la plus séduisante de Pétersbourg, * was also there. She had been
married during the previous winter, and being pregnant did not go to any
large gatherings, but only to small receptions. Prince Vasíli's son,
Hippolyte, had come with Mortemart, whom he introduced. The Abbé Morio
and many others had also come.";

            var result = SynonymReplacer.BigBang(inputText).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
