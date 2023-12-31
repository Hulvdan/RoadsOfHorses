from pathlib import Path

from jinja2 import StrictUndefined, Template

SCRIPT_PATH = Path(__file__).relative_to(Path.cwd()).as_posix()
FILENAME_TEMPLATE = "{}Extensions.cs"
TEMPLATE_PATH = "vectors.j2"

DATA = [
    ("Vector2", ("x", "y"), "float"),
    ("Vector2Int", ("x", "y"), "int"),
    ("Vector3", ("x", "y", "z"), "float"),
    ("Vector3Int", ("x", "y", "z"), "int"),
]


def main():
    template_path = Path(__file__).parent / TEMPLATE_PATH
    with open(template_path) as in_file:
        template = Template(in_file.read(), undefined=StrictUndefined)

    for classname, coords, vartype in DATA:
        data = template.render(
            generated_via=SCRIPT_PATH,
            classname=classname,
            coords=coords,
            vartype=vartype,
        )

        out_file = Path(__file__).parent / FILENAME_TEMPLATE.format(classname)
        with open(out_file, "w") as out_file:
            out_file.write(data)
            out_file.write("\n")


if __name__ == "__main__":
    main()
