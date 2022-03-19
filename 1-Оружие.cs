using System;

class Weapon
{
    private int _damage;
    private int _bullets;
    private int _bulletsPerShot;

    public bool NotEnoughAmmo => _bullets < _bulletsPerShot;

    public Weapon(int damage, int bullets, int bulletsPerShot)
    {
        if (damage < 0)
            throw new ArgumentOutOfRangeException("damage < 0!");

        if (bullets < bulletsPerShot)
            throw new ArgumentOutOfRangeException("Патронов меньше, чем требуется на выстрел!");

        if (bulletsPerShot <= 0)
            throw new ArgumentOutOfRangeException("bulletsPerShot <= 0!");

        _damage = damage;
        _bullets = bullets;
        _bulletsPerShot = bulletsPerShot;
    }

    public void PullTrigger(Player player)
    {
        if (_weapon.NotEnoughAmmo)
            return;

        _bullets -= _bulletsPerShot;
        player.TakeDamage(_damage);
    }
}

class Player
{
    private int _health;

    public bool IsDead => _health <= 0;

    public Player(int health)
    {
        if (health <= 0)
            throw new ArgumentOutOfRangeException("health <= 0!");

        _health = health;
    }

    public void TakeDamage(int damage)
    {
        if (damage > _health)
            damage = _health;

        _health -= damage;
    }
}

class Bot
{
    private Weapon _weapon;

    public Bot(Weapon weapon)
    {
        if (weapon == null)
            throw new NullReferenceException("Оружие не назначено");

        _weapon = weapon;
    }

    private void OnSeePlayer(Player player)
    {
        if (player == null)
            throw new NullReferenceException(nameof(player));

        TryShoot(player);
    }

    private void TryShoot(Player player)
    {
        if (player.IsDead)
            return;

        _weapon.PullTrigger(player);
    }
}