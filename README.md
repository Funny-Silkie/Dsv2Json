# Dsv2Json

- [Dsv2Json](#dsv2json)
  - [Usage](#usage)
    - [Supported Type](#supported-type)
    - [Example](#example)

Tool to convert DSV format file to JSON file.

## Usage

- `-i, --in` option to specify input DSV file (default is stdin).
- `-o, --out` option to specify output JSON file (default is stdout).
- `-d, --delimiter` option to specify the row delimiter character (default is comma: `,`).
- `-t, --type-annotaion` option to use 2nd row as type information.

### Supported Type

| Type Name |     .NET Type     |
| --------: | :---------------: |
|       int |       Int32       |
|      int? | Nullable\<Int32>  |
|      long |       Int64       |
|     long? | Nullable\<Int64>  |
|     float |      Double       |
|    float? | Nullable\<Double> |
|      bool |      Boolean      |
|    string |      String       |

### Example

Convert `src/data/source.csv`.
```shell-session
$ dsv2json -i src/data/source.csv
[
{
  "Name": "Taro",
  "Index": "1",
  "Power": "49",
  "Luck": "16"
},
{
  "Name": "Sabur\u0027o,T",
  "Index": "",
  "Power": "59\u0022",
  "Luck": "58"
},
{
  "Name": "Hanako",
  "Index": "3,3",
  "Power": "\u00221,2\u0022",
  "Luck": "3"
}
]
```

Convert `src/data/typed_source.csv` with type annotations.
```shell-session
$  dsv2json -i src/data/typed_source.csv -t
[
{
  "Name": "None",
  "Index": 0,
  "Power": 0,
  "Luck": 0,
  "Deleted": false
},
{
  "Name": "Taro",
  "Index": 1,
  "Power": 49.5,
  "Luck": 16,
  "Deleted": true
},
{
  "Name": "Saburo",
  "Index": 2,
  "Power": 112,
  "Luck": 58,
  "Deleted": false
}
]s
```
