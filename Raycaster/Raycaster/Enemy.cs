using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raycaster
{
    internal class Enemy : Entity
    {
        public int Health = 3;

        private float HitFrameTime;

        public bool Dead = false;
        private float deathTimer = 1.25f/2;
        private readonly Dictionary<string, SoundEffect> _soundEffects;

        public Enemy(Texture2D texture, Vector2 position, Dictionary<string, SoundEffect> soundEffects, float angle = 0) : base(texture, position, angle)
        {
            _soundEffects = soundEffects;
        }

        public override void Update(float time, Camera camera)
        {
            base.Update(time, camera);

            
            if (Dead)
            {
                if (deathTimer > 0)
                {
                    IsStaticSprite = true;
                    deathTimer -= time;
                    if (deathTimer > 1f/2)
                    {
                        StaticSprite = 41;
                    }else
                    if (deathTimer > 0.75f/2)
                    {
                        StaticSprite = 42;
                    }else
                    if (deathTimer > 0.5f/2)
                    {
                        StaticSprite = 43;
                    }else if (deathTimer > 0.25f/2) 
                    {
                        StaticSprite = 44;
                    }
                    else
                    {
                        StaticSprite = 45;
                    }

                }
                else
                {
                    Active = false;
                }
            }
            else
            {
                if (HitFrameTime > 0)
                {
                    HitFrameTime -= time;

                }
                else
                {
                    IsStaticSprite = false;
                }
            }
        }

        public void damage(int damage)
        {
            if (Dead) return;

            Health -= damage;
            HitFrameTime = 0.7f;
            StaticSprite = 47;
            IsStaticSprite = true;
            if (Health <= 0)
            {
                Dead = true;
                Speed = 0;
                _soundEffects["WolfensteinDeath"].Play();
            }
            else
            {
                _soundEffects["WolfensteinHit"].Play();
            }
            Debug.WriteLine("Health: " +  Health);
        }
    }
}
