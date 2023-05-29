CREATE KEYSPACE ecommerce WITH replication =
  {'class': 'SimpleStrategy', 'replication_factor' : 1};

CREATE TYPE address (
  id uuid,
  street text,
  building_number text,
  house_number text,
  postal_code text,
  city text,
  country text
);

CREATE TYPE payment_detail (
  netto_price decimal,
  brutto_price decimal,
  tax decimal,
  payment_method text,
  status text,
);

CREATE TYPE shipping_method (
  name text,
  price decimal
);

CREATE TABLE IF NOT EXISTS users_by_id (
  user_id uuid,
  email text,
  password text,
  salt text,
  role text,
  name text,
  surname text,
  addresses set<frozen<address>>,
  PRIMARY KEY (user_id)
);

CREATE TABLE IF NOT EXISTS users_by_email (
  email text,
  user_id uuid,
  password text,
  salt text,
  role text,
  PRIMARY KEY (email)
);

CREATE TABLE IF NOT EXISTS products_by_id (
  product_id uuid,
  name text,
  description text,
  price decimal,
  amount_in_stock int,
  categories set<text>,
  image blob,
  PRIMARY KEY (product_id)
);

CREATE TABLE IF NOT EXISTS products_by_name (
  name text,
  product_id uuid,
  description text,
  price decimal,
  amount_in_stock int,
  categories set<text>,
  image blob,
  PRIMARY KEY ((name), product_id)
);

CREATE TABLE IF NOT EXISTS products_in_user_cart(
  user_id uuid,
  product_id uuid,
  product_name text,
  product_description text,
  product_price decimal,
  quantity int,
  image blob, 
  PRIMARY KEY((user_id), product_id)
);

CREATE TABLE IF NOT EXISTS orders_by_id (
  order_id uuid,
  user_id uuid,
  status text static,
  product_id uuid,
  product_name text,
  product_description text,
  product_price decimal,
  product_quantity int,
  payment_details frozen<payment_detail> static,
  shipping_address frozen<address> static,   
  shipping_method frozen<shipping_method> static,
  PRIMARY KEY ((order_id), product_id)
);

CREATE TABLE IF NOT EXISTS orders_by_user (
  user_id uuid,
  order_id uuid, 
  status text,
  created_at timestamp,
  updated_at timestamp,
  PRIMARY KEY ((user_id), order_id, created_at, status)
) WITH CLUSTERING ORDER BY (order_id ASC, created_at DESC, status ASC);

CREATE INDEX orders_by_user_status_idx ON orders_by_user (status);