using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Executor.Command
{
    /*
    public enum Level : byte
    {
       Low = 0,
       Medium = 1,
       High = 2
    }

    //https://docs.unity.cn/Packages/com.unity.render-pipelines.universal@12.1/manual/quality/quality-settings-through-code.html
    public class Graphic : IServe, IDetect
    {
       public readonly string[] FrameRates;
       private readonly Save _save;

       [Inject]
       public Graphic(Save save)
       {
           _save = save;

           FrameRates = new[] { "30", "60", "90", "120" };
       }

       UniTask IServe.Run()
       {
           return UniTask.CompletedTask;
       }

       UniTask IDetect.Detect()
       {
           if (_save.UserContext.Settings.Graphic > 0) return UniTask.CompletedTask;
           if (_save.UserContext.Settings.FrameRate > 0) return UniTask.CompletedTask;
           switch (SystemInfo.graphicsMemorySize)
           {
               case > 4096:
                   SetQualityLevel(Level.High);
                   SetFrameRate(2);
                   break;
               case > 2048:
                   SetQualityLevel(Level.Medium);
                   SetFrameRate(1);
                   break;
               default:
                   SetQualityLevel(Level.Low);
                   SetFrameRate(0);
                   break;
           }

           return UniTask.CompletedTask;
       }

       public void SetQualityLevel(Level level)
       {
           var userData = _save.UserContext;
           userData.Settings.Graphic = Convert.ToByte(level);
           QualitySettings.SetQualityLevel(Convert.ToInt32(level));
       }

       public void SetFrameRate(int index)
       {
           var userData = _save.UserContext;
           userData.Settings.FrameRate = Convert.ToByte(index);
           Application.targetFrameRate = Convert.ToInt32(FrameRates[Math.Clamp(index, 0, FrameRates.Length - 1)]);
       }
    }
     */
    public class SetGraphicCommand : ICommand
    {
        private readonly short _targetFrameRate;
        private readonly short _targetGraphic;
        private readonly int _initFrameRate;
        private readonly int _initGraphic;

        public SetGraphicCommand(short targetFrameRate, short targetGraphic)
        {
            _targetFrameRate = targetFrameRate;
            _targetGraphic = targetGraphic;
            _initFrameRate = Application.targetFrameRate;
            _initGraphic = QualitySettings.GetQualityLevel();
        }

        public UniTask<Status> Execute()
        {
            QualitySettings.SetQualityLevel(_targetGraphic);
            Application.targetFrameRate = _targetFrameRate;
            return UniTask.FromResult(Status.Success);
        }

        public UniTask<Status> Undo()
        {
            QualitySettings.SetQualityLevel(_initGraphic);
            Application.targetFrameRate = _initFrameRate;
            return UniTask.FromResult(Status.Success);
        }
    }
}