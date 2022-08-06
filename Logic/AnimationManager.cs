using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SteamVR_OculusDash_Switcher.Logic
{
	
  
    #region Animation Management

    /// <summary>
    /// The AnimationManager componant allows you to add animations to your container
    /// <item>
    /// <para></para><term>source</term>
    /// <see href="https://social.msdn.microsoft.com/profile/tergiver/?ws=usercard-mini">Tergiver</see>
    /// </item>
    /// </summary>

    public class AnimationManager : Component
    {
        protected System.ComponentModel.IContainer components = null;
        protected List<Animation> activeAnimations = new List<Animation>();
        protected System.Windows.Forms.Timer timer1;

        public AnimationManager()
        {
            InitializeComponent();
        }

        public AnimationManager(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        protected virtual void InitializeComponent()
        {
            // This componant uses a Timer componant for generating animation pulses

            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            this.timer1.Interval = 50; // 20 times per second is pretty smooth for color changes
        }

        // This is the time interval (in ms) of the animation pulses. It has no effect on
        //  the timing of animations, but can be increased or decreased to adjust how
        //  course or fine the animations run.
        [Description("The time interval of the animation pulses in milliseconds.")]
        [DefaultValue(50)]
        public int PulseInterval
        {
            get { return timer1.Interval; }
            set { timer1.Interval = value; }
        }

        // Returns true if there are currently any animations playing.
        [Browsable(false)]
        public bool Animating { get { return activeAnimations.Count > 0; } }

        // Adding an animation will begin it immediately.
        public void Add(Animation animation)
        {
            if (animation == null)
                throw new ArgumentNullException();
            animation.StartTime = DateTime.Now;
            animation.Begin();
            if (animation.IsComplete)
                animation.End();
            else
                activeAnimations.Add(animation);
        }

        // The only way to abort an individual animation is to call the Remove method here.
        public void Remove(Animation animation)
        {
            if (animation == null)
                throw new ArgumentNullException();
            if (activeAnimations.Contains(animation))
            {
                activeAnimations.Remove(animation);
                animation.End();
            }
        }

        // Stops all animations.
        public void Clear()
        {
            foreach (Animation animation in activeAnimations)
                animation.End();
            activeAnimations.Clear();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (activeAnimations.Count > 0)
            {
                List<Animation> deletions = null;

                // Enumerate all active animations and invoke their Pulse method
                foreach (Animation animation in activeAnimations)
                {
                    TimeSpan elapsed = DateTime.Now - animation.StartTime;
                    animation.Pulse(elapsed);
                    // If the animation has run its course, end it and add it to the deletion list
                    if (animation.IsComplete)
                    {
                        animation.End();
                        // Creating the deletion list here so that pulses occurring when there
                        //  are no deletions incur minimal cost
                        if (deletions == null)
                            deletions = new List<Animation>();
                        deletions.Add(animation);
                    }
                }

                // We use a separate list for deletions to simplify enumerating and deleting which is not
                //  allowed on the same list object.
                if (deletions != null)
                    foreach (Animation animation in deletions)
                        activeAnimations.Remove(animation);
            }
        }
    }


    // An animation is anything that has a beginning, "runs" for a period of time (receiving periodic
    //  pulses), and then ends.

    public abstract class Animation
    {
        // Time the animation began. This is set by the AnimationManager.
        public DateTime StartTime { get; internal set; }

        // Has the animation run its course? The animation itself is responsible for setting this to true.
        public bool IsComplete { get; protected set; }
    
        // Called by the AnimationManager when the animation is started. If IsComplete is set to true here
        //  the End method will be called and the Pulse method will never be called. Handy if your "animation"
        //  has a time duration of zero.
        public abstract void Begin();
    
        // Called by the AnimationManager periodically during the animation.
        public abstract void Pulse(TimeSpan elapsed);
    
        // Called by the AnimationManager to signal the animation is done. It is important that
        //  the animation make a final state change to the object it is animating here. That is to say, set
        //  the object's state to be what it should be at the end of the animation.
        public abstract void End();
    }


    // An AnimationSequence is an ordered list of animations. Each executes completely, one after the other.

    public sealed class AnimationSequence : Animation
    {
        private Queue<Animation> queue;
        private Animation currentAnimation = null;

        public AnimationSequence(params Animation[] animations)
        {
            if (animations == null)
                throw new ArgumentNullException();
            queue = new Queue<Animation>(animations);
        }

        public override void Begin()
        {
            // This "animation" is complete, only after all of the animations in the list complete.
            // The odd logic here is in the case of an animation calling itself complete after
            //  we call Begin (a zero-length animation), so we have to return to the queue for
            //  the next one.

        TryNext:
            if (queue.Count == 0)
                IsComplete = true;
            else
            {
                currentAnimation = queue.Dequeue();
                currentAnimation.StartTime = DateTime.Now;
                currentAnimation.Begin();
                if (currentAnimation.IsComplete)
                {
                    currentAnimation.End();
                    goto TryNext;
                }
            }
        }

        public override void Pulse(TimeSpan elapsed)
        {
            // This should not fire because we should not get a Pulse call if we have no running animation.
            Debug.Assert(currentAnimation != null, "currentAnimation != null");

            // Fire off the pulse for the current animation. Note that we pass an elapsed
            //  time of Now minus the animation's start time, not the elapsed value passed
            // to us which is the amount of time elapsed for the entire sequence (so far).
            elapsed = DateTime.Now - currentAnimation.StartTime;
            currentAnimation.Pulse(elapsed);

            // If the current animation has completed
            if (currentAnimation.IsComplete)
            {
                // Fire off its End
                currentAnimation.End();
                currentAnimation = null;
                // Queue up next animation
                Begin();
            }
        }

        public override void End()
        {
            // If End is called (on us) before all of our animations have ended,
            //  we need to call End on each of the remaining ones so that they have the
            //  opportunity to set their animation objects to their final state.
            while (currentAnimation != null)
            {
                currentAnimation.End();
                currentAnimation = queue.Count > 0 ? queue.Dequeue() : null;
            }
        }
    }
    #endregion

    #region Animations


    // DelayAnimation is an animation that does nothing but delay for the specified amount of time.

    public class DelayAnimation : Animation
    {
        TimeSpan delay;
        public DelayAnimation(TimeSpan delay)
        {
            this.delay = delay;
        }

        public override void Begin()
        {
        }

        public override void Pulse(TimeSpan elapsed)
        {
            if (elapsed >= delay)
                IsComplete = true;
        }

        public override void End()
        {
        }
    }


    // A PropertyAnimation allows any (non-indexed.. for now) object property to be animated
    // using an IAnimator object

    public class PropertyAnimation : Animation
    {
        protected object instance;
        protected object startValue, endValue;
        protected string propertyName;
        protected PropertyInfo propertyInfo;
        protected IAnimator animator;
        protected TimeSpan duration;

        // This ctor creates a zero-length animation of the property change (i.e. it'll change
        //  immediately). This is useful if you want to change a property during an AnimationSequence.
        public PropertyAnimation(string propertyName, object instance, object endValue)
            : this(propertyName, instance, endValue, TimeSpan.Zero, null)
        {
        }

        // Create an animation on an object's property. The animator determains how the property
        //  morphs over time.
        public PropertyAnimation(string propertyName, object instance, object endValue, TimeSpan duration, IAnimator animator)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");
            if (instance == null)
                throw new ArgumentNullException("instance");
            this.propertyName = propertyName;
            this.instance = instance;
            this.propertyInfo = instance.GetType().GetProperty(propertyName);
            // Assure that the endValue passed is of the correct type to be set to the target property
            this.endValue = Convert.ChangeType(endValue, propertyInfo.PropertyType);
            this.animator = animator;
            this.duration = duration;
        }

        public override void Begin()
        {
            // If no animator is specified or the duration is zero, flag ourselves complete so
            //  that Pulse is not called.
            if (animator == null || duration == TimeSpan.Zero)
                IsComplete = true;
            // Otherwise we cache the initial value of the property for use during Pulse
            else
                this.startValue = propertyInfo.GetValue(instance, null);
        }

        public override void Pulse(TimeSpan elapsed)
        {
            // If time has expired, we are done
            if (elapsed >= duration)
                IsComplete = true;
            // Otherwise set the property to an intermediate value, determined by the Animator
            else
            {
                // Calculate the "percentage done" for this pulse
                float fraction = (float)elapsed.Ticks / (float)duration.Ticks;

                // Let the Animator do the work of generating an intermediate value
                object temp = animator.ComputeValue(startValue, endValue, fraction);

                // Assure that the Animator's return value is of a compatible type
                temp = Convert.ChangeType(temp, propertyInfo.PropertyType);

                // Change the property
                propertyInfo.SetValue(instance, temp, null);
            }
        }

        public override void End()
        {
            // Change the property to its final value
            propertyInfo.SetValue(instance, endValue, null);
        }
    }
    #endregion


    #region Animators


    // IAnimator is the workhorse for animating values

    public interface IAnimator
    {
        // ComputeValue will receive a starting value, ending value, and a fraction
        //  representing a point between the two.
        // If given an amount of 0.0, this function is expected to return the startValue.
        // If given an amount of 1.0, this function is expected to return the endValue.
        // If given an amount between 0.0 and 1.0, this function should return a value that
        //  represents some appropriate "distance" between the two.
        object ComputeValue(object startValue, object endValue, float amount);
    }


    // The ColorAnimator returns a linear interpolation between two color values representing
    //  the distance between the two.

    public class ColorAnimator : IAnimator
    {
        public object ComputeValue(object startValue, object endValue, float amount)
        {
            Color color1 = (Color)startValue;
            Color color2 = (Color)endValue;
            int r1 = color1.R;
            int g1 = color1.G;
            int b1 = color1.B;
            int r2 = color2.R;
            int g2 = color2.G;
            int b2 = color2.B;
            int n = (int)MathHelper.ClampAndRound(65536f * amount, 0f, 65536f);
            int r3 = r1 + (((r2 - r1) * n) >> 16);
            int g3 = g1 + (((g2 - g1) * n) >> 16);
            int b3 = b1 + (((b2 - b1) * n) >> 16);

            return Color.FromArgb(r3, g3, b3);
        }
    }

    // The DoubleAnimator returns a linear interpolation between two double values.
    public class DoubleAnimator : IAnimator
    {
        public object ComputeValue(object startValue, object endValue, float amount)
        {
            double s = Convert.ToDouble(startValue);
            double e = Convert.ToDouble(endValue);
            int n = (int)MathHelper.ClampAndRound(65536f * amount, 0f, 65536f);
            double r = s + (((e - s) * n) / 65536);
            return r;
        }
    }

    // The IntegerAnimator returns a linear interpolation between two integer values.
    public class IntegerAnimator : IAnimator
    {
        public object ComputeValue(object startValue, object endValue, float amount)
        {
            int s = Convert.ToInt32(startValue);
            int e = Convert.ToInt32(endValue);
            int n = (int)MathHelper.ClampAndRound(65536f * amount, 0f, 65536f);
            int r = s + (((e - s) * n) >> 16);
            return r;
        }
    }
    #endregion

    #region Math Helper
    internal static class MathHelper
    {
        // Clamps the value to min and max while also rounding the value.
        internal static double ClampAndRound(float value, float min, float max)
        {
            if (float.IsNaN(value))
                return 0.0;
            if (float.IsInfinity(value))
                return (float.IsNegativeInfinity(value) ? ((double)min) : ((double)max));
            if (value < min)
                return (double)min;
            if (value > max)
                return (double)max;
            return Math.Round((double)value);
        }
    }
    #endregion
}
