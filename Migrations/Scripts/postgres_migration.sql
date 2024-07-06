CREATE TABLE bot_user
(
    id         SERIAL PRIMARY KEY,
    discord_id VARCHAR(64) UNIQUE NOT NULL
);

CREATE TYPE exercise_type as ENUM ('set', 'speed', 'time');

CREATE TABLE exercise
(
    id          SERIAL PRIMARY KEY,
    name        VARCHAR(100)  NOT NULL,
    type        exercise_type NOT NULL,
    bot_user_id INT           NOT NULL REFERENCES bot_user (id)
);

CREATE TABLE
    exercise_log
(
    id          SERIAL PRIMARY KEY,
    exercise_id INT NOT NULL REFERENCES exercise (id)
);

CREATE TABLE
    all_set_detail
(
    id INT PRIMARY KEY REFERENCES exercise_log (id)
);

CREATE TABLE
    set_exercise_detail
(
    id                SERIAL PRIMARY KEY,
    reps              INT     NOT NULL,
    weight            INT     NOT NULL,
    is_warmup         BOOLEAN NOT NULL,
    all_set_detail_id INT     NOT NULL REFERENCES all_set_detail (id)
);

CREATE TABLE
    speed_exercise_detail
(
    id              INT PRIMARY KEY REFERENCES exercise_log (id),
    duration_sec    INT  NOT NULL,
    distance_miles  REAL NOT NULL,
    exercise_log_id INT  NOT NULL
);

CREATE TABLE
    time_exercise_details
(
    id              INT PRIMARY KEY REFERENCES exercise_log (id),
    duration_sec    INT NOT NULL,
    exercise_log_id INT NOT NULL
);
