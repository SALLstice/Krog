select = 0

menuItems = ["Attack", "Tactics", "Items"]

while 1:
    # actionMenu = "0 Attack\n1 Tactics\n2 Items"
    # actionMenu = actionMenu.replace(select,">")

    for idx, val in enumerate(menuItems):
        if idx == select:
            print(f"> {val}")
        else:
            print(f"  {val}")

    select = int(input())
