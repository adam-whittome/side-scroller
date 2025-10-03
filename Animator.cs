using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSCRefactor
{
    public class Frame
    {
        public Texture2D Texture;
        public int OffsetX;
        public int OffsetY;
        public float Duration;
        public Frame(Texture2D texture, int offsetX, int offsetY, float duration)
        {
            Texture = texture;
            OffsetX = offsetX;
            OffsetY = offsetY;
            Duration = duration;
        }
    }
    public class Animator
    {
        private Dictionary<string, (Frame[], string)> animation;
        public string TargetAnimation;
        private string previousAnimation;
        private float timeElapsed;
        private int currentFrame;
        public Animator(Dictionary<string, (Frame[], string)> animation, string initialAnimation)
        {
            this.animation = animation;
            TargetAnimation = initialAnimation;
            timeElapsed = 0;
            currentFrame = 0;
        }
        public Frame GetCurrentFrame(float deltaTime)
        {
            timeElapsed += deltaTime;
            if (previousAnimation != TargetAnimation) currentFrame = 0;
            previousAnimation = TargetAnimation;
            (Frame[] frameArray, string nextAnimation) = animation[TargetAnimation];
            if (timeElapsed > frameArray[currentFrame].Duration)
            {
                timeElapsed -= frameArray[currentFrame].Duration;
                currentFrame++;
                if (currentFrame >= frameArray.Length)
                {
                    currentFrame = 0;
                    TargetAnimation = nextAnimation;
                }
            }
            return frameArray[currentFrame];
        }
    }
}
