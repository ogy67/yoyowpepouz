using UnityEngine;

public class collisionSouris : MonoBehaviour
{
    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    /*private void FixedUpdate()
    {
        // quand colision non trigger avec obstacle, position n'est plus celle de player
        transform.position = _player.transform.position;

    }

    public playerMovment _player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Obstacle" && collision.gameObject.tag != "ObstacleBounce")
            return;
        _player.sourisCollision_death();
    }*/
    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag != "Obstacle" && collision.gameObject.tag != "ObstacleBounce"))
            return;
    }*/
}
