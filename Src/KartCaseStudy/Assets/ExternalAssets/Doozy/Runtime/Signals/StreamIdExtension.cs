// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

//.........................
//.....Generated Class.....
//.........................
//.......Do not edit.......
//.........................

using UnityEngine;
// ReSharper disable All

namespace Doozy.Runtime.Signals
{
    public partial class Signal
    {
        public static bool Send(StreamId.GameplayStream id, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), message);
        public static bool Send(StreamId.GameplayStream id, GameObject signalSource, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalSource, message);
        public static bool Send(StreamId.GameplayStream id, SignalProvider signalProvider, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalProvider, message);
        public static bool Send(StreamId.GameplayStream id, Object signalSender, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalSender, message);
        public static bool Send<T>(StreamId.GameplayStream id, T signalValue, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalValue, message);
        public static bool Send<T>(StreamId.GameplayStream id, T signalValue, GameObject signalSource, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalValue, signalSource, message);
        public static bool Send<T>(StreamId.GameplayStream id, T signalValue, SignalProvider signalProvider, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalValue, signalProvider, message);
        public static bool Send<T>(StreamId.GameplayStream id, T signalValue, Object signalSender, string message = "") => SignalsService.SendSignal(nameof(StreamId.GameplayStream), id.ToString(), signalValue, signalSender, message);

        public static bool Send(StreamId.MainMenuStream id, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), message);
        public static bool Send(StreamId.MainMenuStream id, GameObject signalSource, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalSource, message);
        public static bool Send(StreamId.MainMenuStream id, SignalProvider signalProvider, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalProvider, message);
        public static bool Send(StreamId.MainMenuStream id, Object signalSender, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalSender, message);
        public static bool Send<T>(StreamId.MainMenuStream id, T signalValue, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalValue, message);
        public static bool Send<T>(StreamId.MainMenuStream id, T signalValue, GameObject signalSource, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalValue, signalSource, message);
        public static bool Send<T>(StreamId.MainMenuStream id, T signalValue, SignalProvider signalProvider, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalValue, signalProvider, message);
        public static bool Send<T>(StreamId.MainMenuStream id, T signalValue, Object signalSender, string message = "") => SignalsService.SendSignal(nameof(StreamId.MainMenuStream), id.ToString(), signalValue, signalSender, message);
   
    }

    public partial class StreamId
    {
        public enum GameplayStream
        {
            OnResumeButton
        }

        public enum MainMenuStream
        {
            BackButtonPressed,
            OpenMainMenu
        }         
    }
}

