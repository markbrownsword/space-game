using UnityEngine;
using Unity.Entities;

public class MoveSystem : ComponentSystem
{
    public struct Data
    {
        public readonly int Length;
        public EntityArray Entities;
        public GameObjectArray GameObjects;
        public ComponentArray<MoveComponent> MoveComponents;
        public ComponentArray<Rigidbody2D> rb2ds;
        public ComponentArray<Animator> animators;
    }

    [Inject] private Data data;

    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            var gameObject = data.GameObjects[i];
            var moveComponent = data.MoveComponents[i];
            var rb2d = data.rb2ds[i];

            var inputX = 0.0f;
            var inputY = 0.0f;

            if (gameObject.tag == "Player")
            {
                inputX = Input.GetAxis("Horizontal") * moveComponent.speed;
                inputY = rb2d.velocity.y;
            }
            else
            {
                Debug.Log("Enemy!");
            }

            rb2d.velocity = new Vector2(inputX, inputY);
        }
    }
}
