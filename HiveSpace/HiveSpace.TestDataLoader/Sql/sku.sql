DELETE FROM skus;

DROP TABLE if EXISTS temp_result;

CREATE TEMP TABLE temp_result AS
SELECT
    DENSE_RANK() OVER (
        PARTITION BY
            "ProductId"
        ORDER BY "Name"
    ) AS rank_num,
    "ProductId",
    value,
    "Name"
FROM product_variants, jsonb_array_elements(product_variants."Options") AS value;

insert into
    skus (
        "Id",
        "SkuNo",
        "ProductId",
        "SkuVariants",
        "Quantity",
        "InActive",
        "Amount",
        "Currency"
    )
SELECT
    ROW_NUMBER() OVER (
        ORDER BY tr1."ProductId"
    ),
    CONCAT(
        CHR(
            FLOOR(RANDOM() * 26 + 65)::INT
        ),
        CHR(
            FLOOR(RANDOM() * 26 + 65)::INT
        ),
        CHR(
            FLOOR(RANDOM() * 26 + 65)::INT
        ),
        FLOOR(RANDOM() * 10)::INT,
        FLOOR(RANDOM() * 10)::INT,
        FLOOR(RANDOM() * 10)::INT
    ) AS random_sku,
    tr1."ProductId",
    jsonb_build_array(
        jsonb_build_object(
            'name',
            tr1."Name",
            'value',
            tr1."value"
        ),
        jsonb_build_object(
            'name',
            tr2."Name",
            'value',
            tr2."value"
        )
    ) AS conbination,
    FLOOR(RANDOM() * 100) as quantity,
    cast(1 AS BOOLEAN),
    FLOOR(
        (
            FLOOR(
                RANDOM() * (5000000 - 10000 + 1)
            ) + 10000
        ) / 1000
    ) * 1000,
    'VND'
FROM
    temp_result tr1
    JOIN temp_result tr2 on tr1."ProductId" = tr2."ProductId"
    and tr1."Name" <> tr2."Name"
    and tr1."rank_num" < tr2."rank_num"
GROUP BY
    tr1."rank_num",
    tr1."ProductId",
    tr1."value",
    tr1."Name",
    tr2."rank_num",
    tr2."ProductId",
    tr2."value",
    tr2."Name"