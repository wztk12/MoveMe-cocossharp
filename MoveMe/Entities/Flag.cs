using CocosSharp;

namespace MoveMe.Entities
{
    class Flag : AnimatedEntity
    {
        Animation wave;
        public Flag()
        {
            wave = GetAnimation("flag");
            defaultSprite = new CCSprite(wave.Frames[0]);
            sprite = defaultSprite;
            this.sprite.Scale = 2f;
            this.sprite.AddAction(wave.Action);
        }

    }
}