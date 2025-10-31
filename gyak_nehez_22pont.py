"""
Egy egy adat osztály (mondhatjuk struktúrának, rekordnak, stb), ami azért előnyös, hogy típusosan el tudjam tárolni az adatokat.
A játék minden körben koordinátákat ad, ami áll egy x-ből és egy y-ból. Enélkül az osztály nélkül lista indexekre kéne hagyatkoznom,
ami nem előnyös. Így viszont ahogy a későbbiekben is látszik, tudok olyat mondani, hogy player_position.x, ami sokkal érthetőbbé teszi a kódot
"""
class Position:
    x: int
    y: int

    """
    Ez az ún. konstruktor, ami egy speciális függvény, ami egy osztály inicializálásakor fut le. Jelen esetben megkaptja stringként a 
    koordinátákat, felbontja, majd belerakja x és y változókba
    """
    def __init__(self, pos: str):
        coordinates = pos.split(";")
        self.x = int(coordinates[0])
        self.y = int(coordinates[1])

"""
Ez a függvény egy egyszerű mohó algoritmust valósít meg. 

A mohó algoritmusok lényege, hogy minden körben azt lépik, ami éppen az aktuális legjobbnak gondolt lépés, figyelembe nem véve azt, hogy a játék 
egészére nézve optimális lépés-e

Jelen esetben minden körben megkeresem a legközelebbi őrt és felé teszek egy lépést, figyelve arra, hogy kikerüljem a középső mezőt.
"""
def vedekezes(player_position: str, guard_positions: list[str], remaining: int):
    # játékos pozíciójának átalakítása a Position objektummá
    player_position: Position = Position(player_position)
    # őrök pozícióinak átalakítása Position objektumokká list comprehension segítségével
    guard_positions: list[Position] = [Position(pos) for pos in guard_positions]

    # legközelebbi őr megkeresése
    closest = get_closest_guard(player_position, guard_positions)

    # legközelebbi őr felé lépés
    return move_to_closest(player_position, closest)

"""
Ez a függvény megkeresi a legközelebbi őrt. Ez egy egyszerű matek probléma, vektoroknak értelmezve a koordinátákat
kiszámolom a játékos és az őrök közti távolságot, majd megkeresem a legkisebbet
"""
def get_closest_guard(player_position: Position, guard_positions: list[Position]):
    # Értelmes nyelveken min keresésekor a kezdőérték a legnagyobb olyan érték, ami a változóbal eltárolható, 
    # hogy ugye annál minden kisebb. Pythonban nem lehet package nélkül meghatározni egy változó max értékét.
    # Viszont mivel 9x9-es a board, ezért a legnagyobb távolság is kisebb lesz, mint 100, ha jól tudok számolni.
    min_distance: float = 100

    # Legközelebbi őr kezdőértéke. Értelmes nyelvekben ez null lenne, python viszont nem szeretni, ha None értéket kap egy változó
    closest: Position = player_position

    for pos in guard_positions:
        # távolság kiszámítása Pythagoras tétellel
        distance: float = ((pos.x - player_position.x) ** 2) + ((pos.y - player_position.y) ** 2) ** 0.5

        # klasszik min keresés
        if distance < min_distance:
            min_distance = distance
            closest = pos

    return closest

"""
Ez a föggvény megkapja a játékost, és a legközelebbi őr helyzetét, majd meghatározza, hogy merre kéne lépni.
Note: itt ugye lehet átlósan is lépni, azért van még az x ellenőrzése után y, hogy ha kell, akkor kiegészítse
"""
def move_to_closest(player_position: Position, closest: Position):
    move: str = ""

    if closest.x < player_position.x:
        move += "b"
    if closest.x > player_position.x:
        move += "j"

    if closest.y < player_position.y:
        move += "l"
    if closest.y > player_position.y:
        move += "f"

    return corrigate_center(player_position, move)

"""
Nem életem legszebb függvénye, de ez arra hivatott, hogy kikerülje az 0;0-át.
Ezt úgy csinálja, hogy az előzőleg meghatároztt lépés alapján megnézi, hogy ha arra lépne a játékos,
akkor a 0;0-án landolna-e, és ha igen, akkor random irányba ellép. Pl ha balra kéne lépni, de balra 
a közzéppont van, akkor balra-fel lép inkább
"""
def corrigate_center(player_position: Position, move: str):
    if move == "b" and player_position.x - 1 == 0 and player_position.y == 0:
        return "bf"
    elif move == "bf" and player_position.x - 1 == 0 and player_position.y + 1 == 0:
        return "b"
    elif move == "f" and player_position.x == 0 and player_position.y + 1 == 0:
        return "jf"
    elif move == "jf" and player_position.x + 1 == 0 and player_position.y + 1 == 0:
        return "j"
    elif move == "j" and player_position.x + 1 == 0 and player_position.y == 0:
        return "jf"
    elif move == "jl" and player_position.x + 1 == 0 and player_position.y - 1 == 0:
        return "l"
    elif move == "l" and player_position.x == 0 and player_position.y - 1 == 0:
        return "jl"
    elif move == "bl" and player_position.x - 1 == 0 and player_position.y -1 == 0:
        return "l"
    else:
        return move