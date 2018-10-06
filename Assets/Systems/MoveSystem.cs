using UnityEngine;
using Unity.Entities;

[UpdateAfter(typeof(UnityEngine.Experimental.PlayerLoop.FixedUpdate))]
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
            var inputY = rb2d.velocity.y;

            if (gameObject.tag == "Player")
            {
                inputX = Mathf.Lerp(0, Input.GetAxis("Horizontal") * moveComponent.speed, 1f);
                Debug.Log(string.Format("Player InputX: {0}", inputX));
            }
            else
            {
                Debug.Log("Enemy!");
            }

            rb2d.velocity = new Vector2(inputX, inputY);
        }
    }
}
